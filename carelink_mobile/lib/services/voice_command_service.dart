import 'api_client.dart';

class VoiceCommandService {
  final ApiClient _apiClient = ApiClient();

  Future<ApiResult<Map<String, dynamic>>> processCommand({
    required String patientProfileId,
    required String recognizedText,
  }) async {
    return await _apiClient.post('/api/voicecommand/process', {
      'patientProfileId': patientProfileId,
      'recognizedText': recognizedText,
    });
  }
}
