import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import '../theme/app_theme.dart';
import '../services/gesture_service.dart';
import 'package:flutter/foundation.dart';
import 'dart:typed_data';

class GestureScreen extends StatefulWidget {
  const GestureScreen({super.key});

  @override
  State<GestureScreen> createState() => _GestureScreenState();
}

class _GestureScreenState extends State<GestureScreen> {
  final _gestureService = GestureService();
  final _imagePicker = ImagePicker();

  File? _capturedImage;
  Uint8List? _capturedImageBytes;
  bool _isProcessing = false;
  GestureResult? _lastResult;
  List<String> _errors = [];

  Future<void> _captureAndAnalyze() async {
    final photo = await _imagePicker.pickImage(
      source: ImageSource.camera,
      imageQuality: 85,
    );

    if (photo == null) return;

    setState(() {
      _isProcessing = true;
      _lastResult = null;
      _errors = [];
    });

    GestureResult result;

    if (kIsWeb) {
      final bytes = await photo.readAsBytes();
      setState(() {
        _capturedImageBytes = bytes;
        _capturedImage = null;
      });
      result = await _gestureService.analyzeImage(imageBytes: bytes);
    } else {
      final file = File(photo.path);
      setState(() {
        _capturedImage = file;
        _capturedImageBytes = null;
      });
      result = await _gestureService.analyzeImage(imageFile: file);
    }

    if (!mounted) return;

    setState(() {
      _isProcessing = false;
      if (result.success) {
        _lastResult = result;
      } else {
        _errors = result.errors;
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('إيماءات اليد'),
        backgroundColor: AppColors.ink,
        foregroundColor: Colors.white,
      ),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          children: [
            const Text(
              'التقط صورة ليدك، مثلاً كف مفتوح لتفعيل الطوارئ',
              style: TextStyle(fontSize: 15, color: Colors.black54),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 24),
            if (_capturedImage != null && !kIsWeb)
              ClipRRect(
                borderRadius: BorderRadius.circular(12),
                child: Image.file(
                  _capturedImage!,
                  height: 220,
                  fit: BoxFit.cover,
                ),
              ),
            if (_capturedImage != null && kIsWeb)
              Container(
                height: 220,
                decoration: BoxDecoration(
                  color: AppColors.border,
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Center(
                  child: Icon(
                    Icons.check_circle,
                    size: 48,
                    color: AppColors.calm,
                  ),
                ),
              ),
            const SizedBox(height: 24),
            ElevatedButton.icon(
              onPressed: _isProcessing ? null : _captureAndAnalyze,
              icon: const Icon(Icons.camera_alt),
              label: Text(_isProcessing ? 'جاري التحليل...' : 'التقاط صورة'),
            ),
            const SizedBox(height: 24),
            if (_isProcessing) const CircularProgressIndicator(),
            if (_lastResult != null) _buildResultCard(_lastResult!),
            if (_errors.isNotEmpty)
              Text(
                _errors.join('\n'),
                style: const TextStyle(color: AppColors.critical),
                textAlign: TextAlign.center,
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildResultCard(GestureResult result) {
    if (!result.detected) {
      return Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: AppColors.border,
          borderRadius: BorderRadius.circular(10),
        ),
        child: const Text(
          'لم يتم رصد أي يد واضحة في الصورة، حاول مرة أخرى بإضاءة أفضل.',
          textAlign: TextAlign.center,
        ),
      );
    }

    final gestureLabel = switch (result.gesture) {
      'OpenPalm' => 'كف مفتوح — تم تفعيل الطوارئ',
      'ClosedFist' => 'قبضة مغلقة — طلب الاتصال بمقدم الرعاية',
      'Victory' => 'علامة النصر — تذكير بالدواء',
      _ => 'إيماءة غير معروفة',
    };

    final color = result.gesture == 'OpenPalm'
        ? AppColors.critical
        : AppColors.calm;

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color),
      ),
      child: Column(
        children: [
          Text(
            gestureLabel,
            style: TextStyle(
              color: color,
              fontWeight: FontWeight.bold,
              fontSize: 15,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 6),
          Text(
            'نسبة الثقة: ${(result.confidence * 100).toStringAsFixed(0)}%',
            style: const TextStyle(fontSize: 12, color: Colors.black54),
          ),
        ],
      ),
    );
  }
}
