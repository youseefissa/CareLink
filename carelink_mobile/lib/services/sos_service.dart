import 'api_client.dart';
import 'token_storage_service.dart';

class SosService {
  final ApiClient _apiClient = ApiClient();
  final TokenStorageService _tokenStorage = TokenStorageService();

  Future<ApiResult<Map<String, dynamic>>> triggerSos({
    required String patientProfileId,
    required String triggerSource,
    double? latitude,
    double? longitude,
  }) async {
    return await _apiClient.post('/api/sos/trigger', {
      'patientProfileId': patientProfileId,
      'triggerSource': triggerSource,
      'latitude': latitude,
      'longitude': longitude,
    });
  }
}
