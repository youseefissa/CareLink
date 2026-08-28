import 'package:flutter/material.dart';
import '../theme/app_theme.dart';
import '../services/location_service.dart';
import '../services/sos_service.dart';
import '../services/token_storage_service.dart';
import 'status_screen.dart';
import 'voice_command_screen.dart';
import 'gesture_screen.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final _locationService = LocationService();
  final _sosService = SosService();
  final _tokenStorage = TokenStorageService();

  double? _latitude;
  double? _longitude;
  String? _locationError;
  bool _isLoadingLocation = true;
  bool _isSendingSos = false;

  @override
  void initState() {
    super.initState();
    _loadLocation();
  }

  Future<void> _loadLocation() async {
    setState(() {
      _isLoadingLocation = true;
    });

    final result = await _locationService.getCurrentLocation();

    if (!mounted) return;

    setState(() {
      _isLoadingLocation = false;
      if (result.success) {
        _latitude = result.latitude;
        _longitude = result.longitude;
        _locationError = null;
      } else {
        _locationError = result.errorMessage;
      }
    });
  }

  Future<void> _handleSosPress() async {
    setState(() {
      _isSendingSos = true;
    });

    final patientProfileId = await _tokenStorage.getPatientProfileId();

    if (patientProfileId == null) {
      if (!mounted) return;
      setState(() {
        _isSendingSos = false;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('تعذر إرسال الطلب، بيانات المريض غير متوفرة.'),
        ),
      );
      return;
    }

    final result = await _sosService.triggerSos(
      patientProfileId: patientProfileId,
      triggerSource: 'Button',
      latitude: _latitude,
      longitude: _longitude,
    );

    if (!mounted) return;

    setState(() {
      _isSendingSos = false;
    });

    if (result.success) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'تم إرسال طلب الطوارئ بنجاح، سيتم إبلاغ القائم على رعايتك.',
          ),
          backgroundColor: AppColors.calm,
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(result.errors.join('\n')),
          backgroundColor: AppColors.critical,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('CareLink AI'),
        backgroundColor: AppColors.ink,
        foregroundColor: Colors.white,
        actions: [
          IconButton(
            icon: const Icon(Icons.favorite_border),
            onPressed: () {
              Navigator.of(
                context,
              ).push(MaterialPageRoute(builder: (_) => const StatusScreen()));
            },
          ),
        ],
      ),
      body: Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              _buildLocationStatus(),
              const SizedBox(height: 32),
              const Text(
                'اضغط الزرار في حالة الطوارئ',
                style: TextStyle(fontSize: 16, color: Colors.black54),
              ),
              const SizedBox(height: 32),
              GestureDetector(
                onTap: _isSendingSos ? null : _handleSosPress,
                child: Container(
                  width: 180,
                  height: 180,
                  decoration: BoxDecoration(
                    color: _isSendingSos
                        ? AppColors.critical.withValues(alpha: 0.6)
                        : AppColors.critical,
                    shape: BoxShape.circle,
                  ),
                  child: Center(
                    child: _isSendingSos
                        ? const CircularProgressIndicator(color: Colors.white)
                        : const Text(
                            'SOS',
                            style: TextStyle(
                              color: Colors.white,
                              fontSize: 36,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                  ),
                ),
              ),
              const SizedBox(height: 32),
              OutlinedButton.icon(
                onPressed: () {
                  Navigator.of(context).push(
                    MaterialPageRoute(
                      builder: (_) => const VoiceCommandScreen(),
                    ),
                  );
                },
                icon: const Icon(Icons.mic),
                label: const Text('الأوامر الصوتية'),
              ),
              const SizedBox(height: 16),
              OutlinedButton.icon(
                onPressed: () {
                  Navigator.of(context).push(
                    MaterialPageRoute(builder: (_) => const GestureScreen()),
                  );
                },
                icon: const Icon(Icons.pan_tool),
                label: const Text('إيماءات اليد'),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildLocationStatus() {
    if (_isLoadingLocation) {
      return const Text(
        'جاري تحديد موقعك...',
        style: TextStyle(color: Colors.black54, fontSize: 13),
      );
    }

    if (_locationError != null) {
      return Column(
        children: [
          Text(
            _locationError!,
            style: const TextStyle(color: AppColors.critical, fontSize: 13),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 8),
          TextButton(
            onPressed: _loadLocation,
            child: const Text('إعادة المحاولة'),
          ),
        ],
      );
    }

    return const Text(
      'تم تحديد موقعك بنجاح',
      style: TextStyle(color: AppColors.calm, fontSize: 13),
    );
  }
}
