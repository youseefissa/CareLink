import 'api_client.dart';

class PatientService {
  final ApiClient _apiClient = ApiClient();

  Future<ApiResult<Map<String, dynamic>>> getMyProfile() async {
    return await _apiClient.get('/api/patient/me');
  }
}
