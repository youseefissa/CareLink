import 'package:flutter/material.dart';
import 'package:speech_to_text/speech_to_text.dart' as stt;
import '../theme/app_theme.dart';
import '../services/voice_command_service.dart';
import '../services/token_storage_service.dart';

class VoiceCommandScreen extends StatefulWidget {
  const VoiceCommandScreen({super.key});

  @override
  State<VoiceCommandScreen> createState() => _VoiceCommandScreenState();
}

class _VoiceCommandScreenState extends State<VoiceCommandScreen> {
  final _speech = stt.SpeechToText();
  final _voiceCommandService = VoiceCommandService();
  final _tokenStorage = TokenStorageService();

  bool _isListening = false;
  bool _isProcessing = false;
  bool _isSpeechAvailable = false;
  String _recognizedText = '';
  String? _resultMessage;

  @override
  void initState() {
    super.initState();
    _initSpeech();
  }

  Future<void> _initSpeech() async {
    final available = await _speech.initialize(
      onStatus: (status) {
        if (status == 'done' || status == 'notListening') {
          setState(() {
            _isListening = false;
          });
        }
      },
      onError: (error) {
        setState(() {
          _isListening = false;
          _resultMessage = 'حدث خطأ في التعرف على الصوت.';
        });
      },
    );

    setState(() {
      _isSpeechAvailable = available;
    });
  }

  Future<void> _startListening() async {
    setState(() {
      _recognizedText = '';
      _resultMessage = null;
    });

    if (!_isSpeechAvailable) {
      setState(() {
        _resultMessage =
            'التعرف على الصوت غير متاح على هذا الجهاز، تأكد من منح إذن الميكروفون.';
      });
      return;
    }

    setState(() {
      _isListening = true;
    });

    await _speech.listen(
      localeId: 'ar-EG',
      onResult: (result) {
        setState(() {
          _recognizedText = result.recognizedWords;
        });

        if (result.finalResult) {
          _submitCommand(result.recognizedWords);
        }
      },
    );
  }

  Future<void> _stopListening() async {
    await _speech.stop();
    setState(() {
      _isListening = false;
    });
  }

  Future<void> _submitCommand(String text) async {
    if (text.trim().isEmpty) return;

    setState(() {
      _isProcessing = true;
    });

    final patientProfileId = await _tokenStorage.getPatientProfileId();

    if (patientProfileId == null) {
      setState(() {
        _isProcessing = false;
        _resultMessage = 'تعذر إرسال الأمر، بيانات المريض غير متوفرة.';
      });
      return;
    }

    final result = await _voiceCommandService.processCommand(
      patientProfileId: patientProfileId,
      recognizedText: text,
    );

    if (!mounted) return;

    setState(() {
      _isProcessing = false;
      _resultMessage = result.success
          ? 'تم استلام الأمر بنجاح.'
          : result.errors.join('\n');
    });
  }

  @override
  void dispose() {
    _speech.stop();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('الأوامر الصوتية'),
        backgroundColor: AppColors.ink,
        foregroundColor: Colors.white,
      ),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Text(
              'اضغط الزرار وتحدث، مثلاً: ساعدني، أو اتصل بمقدم الرعاية',
              style: TextStyle(fontSize: 15, color: Colors.black54),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 40),
            GestureDetector(
              onTap: _isListening ? _stopListening : _startListening,
              child: Container(
                width: 140,
                height: 140,
                decoration: BoxDecoration(
                  color: _isListening ? AppColors.critical : AppColors.ink,
                  shape: BoxShape.circle,
                ),
                child: Icon(
                  _isListening ? Icons.mic : Icons.mic_none,
                  color: Colors.white,
                  size: 56,
                ),
              ),
            ),
            const SizedBox(height: 32),
            if (_recognizedText.isNotEmpty)
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: AppColors.surface,
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: AppColors.border),
                ),
                child: Text(
                  _recognizedText,
                  style: const TextStyle(fontSize: 15, color: AppColors.ink),
                  textAlign: TextAlign.center,
                ),
              ),
            const SizedBox(height: 16),
            if (_isProcessing) const CircularProgressIndicator(),
            if (_resultMessage != null)
              Text(
                _resultMessage!,
                style: TextStyle(
                  fontSize: 14,
                  color: _resultMessage!.contains('نجاح')
                      ? AppColors.calm
                      : AppColors.critical,
                ),
                textAlign: TextAlign.center,
              ),
          ],
        ),
      ),
    );
  }
}
