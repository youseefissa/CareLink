import 'dart:convert';
import 'dart:io';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import 'token_storage_service.dart';
import 'api_client.dart';

class GestureResult {
  final bool success;
  final String? gesture;
  final double confidence;
  final bool detected;
  final bool wasExecuted;
  final List<String> errors;

  GestureResult({
    required this.success,
    this.gesture,
    this.confidence = 0.0,
    this.detected = false,
    this.wasExecuted = false,
    this.errors = const [],
  });
}

class GestureService {
  final TokenStorageService _tokenStorage = TokenStorageService();

  Future<GestureResult> analyzeImage({
    File? imageFile,
    Uint8List? imageBytes,
  }) async {
    final patientProfileId = await _tokenStorage.getPatientProfileId();
    final token = await _tokenStorage.getToken();

    if (patientProfileId == null || token == null) {
      return GestureResult(
        success: false,
        errors: ['بيانات المستخدم غير متوفرة.'],
      );
    }

    try {
      final uri = Uri.parse(
        '${ApiClient.baseUrl}/api/gesturecommand/analyze-image',
      );
      final request = http.MultipartRequest('POST', uri);

      request.headers['Authorization'] = 'Bearer $token';
      request.fields['patientProfileId'] = patientProfileId;

      if (kIsWeb && imageBytes != null) {
        request.files.add(
          http.MultipartFile.fromBytes(
            'file',
            imageBytes,
            filename: 'gesture.jpg',
          ),
        );
      } else if (imageFile != null) {
        request.files.add(
          await http.MultipartFile.fromPath('file', imageFile.path),
        );
      } else {
        return GestureResult(
          success: false,
          errors: ['لم يتم العثور على صورة صالحة.'],
        );
      }

      final streamedResponse = await request.send();
      final response = await http.Response.fromStream(streamedResponse);

      if (response.statusCode >= 200 && response.statusCode < 300) {
        if (response.body.isEmpty) {
          return GestureResult(success: true, detected: false);
        }

        final decoded = jsonDecode(response.body) as Map<String, dynamic>;

        return GestureResult(
          success: true,
          gesture: decoded['gesture'],
          confidence: (decoded['confidence'] ?? 0.0).toDouble(),
          detected: decoded['detected'] ?? false,
          wasExecuted: decoded['wasExecuted'] ?? false,
        );
      }

      return GestureResult(
        success: false,
        errors: ['فشل تحليل الصورة، حاول مرة أخرى.'],
      );
    } catch (e) {
      return GestureResult(
        success: false,
        errors: ['تعذر الاتصال بالسيرفر: $e'],
      );
    }
  }
}
