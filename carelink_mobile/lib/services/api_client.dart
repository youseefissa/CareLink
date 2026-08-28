import 'dart:convert';
import 'dart:io';
import 'package:http/http.dart' as http;
import 'token_storage_service.dart';

class ApiResult<T> {
  final bool success;
  final T? data;
  final List<String> errors;

  ApiResult({required this.success, this.data, this.errors = const []});
}

class ApiListResult<T> {
  final bool success;
  final List<T>? data;
  final List<String> errors;

  ApiListResult({required this.success, this.data, this.errors = const []});
}

class ApiClient {
  static const String baseUrl = 'https://localhost:7253';
  final TokenStorageService _tokenStorage = TokenStorageService();

  ApiClient() {
    HttpOverrides.global = _DevHttpOverrides();
  }

  Future<Map<String, String>> _buildHeaders({bool withAuth = true}) async {
    final headers = {'Content-Type': 'application/json'};

    if (withAuth) {
      final token = await _tokenStorage.getToken();
      if (token != null) {
        headers['Authorization'] = 'Bearer $token';
      }
    }

    return headers;
  }

  Future<ApiResult<Map<String, dynamic>>> post(
    String endpoint,
    Map<String, dynamic> body, {
    bool withAuth = true,
  }) async {
    try {
      final headers = await _buildHeaders(withAuth: withAuth);

      final response = await http.post(
        Uri.parse('$baseUrl$endpoint'),
        headers: headers,
        body: jsonEncode(body),
      );

      return _parseResponse(response);
    } catch (e) {
      return ApiResult(success: false, errors: ['تعذر الاتصال بالسيرفر: $e']);
    }
  }

  Future<ApiResult<Map<String, dynamic>>> get(String endpoint) async {
    try {
      final headers = await _buildHeaders();

      final response = await http.get(
        Uri.parse('$baseUrl$endpoint'),
        headers: headers,
      );

      return _parseResponse(response);
    } catch (e) {
      return ApiResult(success: false, errors: ['تعذر الاتصال بالسيرفر: $e']);
    }
  }

  Future<ApiListResult<Map<String, dynamic>>> getList(String endpoint) async {
    try {
      final headers = await _buildHeaders();

      final response = await http.get(
        Uri.parse('$baseUrl$endpoint'),
        headers: headers,
      );

      if (response.statusCode >= 200 && response.statusCode < 300) {
        final decoded = jsonDecode(response.body) as List;
        final list = decoded.cast<Map<String, dynamic>>();
        return ApiListResult(success: true, data: list);
      }

      return ApiListResult(
        success: false,
        errors: ['حدث خطأ أثناء تحميل البيانات.'],
      );
    } catch (e) {
      return ApiListResult(
        success: false,
        errors: ['تعذر الاتصال بالسيرفر: $e'],
      );
    }
  }

  ApiResult<Map<String, dynamic>> _parseResponse(http.Response response) {
    if (response.statusCode >= 200 && response.statusCode < 300) {
      if (response.body.isEmpty) {
        return ApiResult(success: true, data: {});
      }

      final decoded = jsonDecode(response.body);
      return ApiResult(success: true, data: decoded);
    }

    try {
      final decoded = jsonDecode(response.body);
      final errors =
          (decoded['errors'] as List?)?.map((e) => e.toString()).toList() ??
          ['حدث خطأ غير متوقع.'];
      return ApiResult(success: false, errors: errors);
    } catch (_) {
      return ApiResult(success: false, errors: ['حدث خطأ غير متوقع.']);
    }
  }
}

class _DevHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return super.createHttpClient(context)
      ..badCertificateCallback = (cert, host, port) => true;
  }
}
