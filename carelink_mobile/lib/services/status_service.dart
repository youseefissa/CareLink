import 'api_client.dart';
import '../models/status_models.dart';

class StatusService {
  final ApiClient _apiClient = ApiClient();

  Future<List<AlertItem>> getAlerts(String patientProfileId) async {
    final result = await _apiClient.getList(
      '/api/alert/patient/$patientProfileId',
    );
    if (!result.success || result.data == null) return [];
    return result.data!.map((e) => AlertItem.fromJson(e)).toList();
  }

  Future<List<RecommendationItem>> getRecommendations(
    String patientProfileId,
  ) async {
    final result = await _apiClient.getList(
      '/api/safetyrecommendation/patient/$patientProfileId',
    );
    if (!result.success || result.data == null) return [];
    return result.data!.map((e) => RecommendationItem.fromJson(e)).toList();
  }

  Future<List<ActivityItem>> getActivityHistory(String patientProfileId) async {
    final result = await _apiClient.getList(
      '/api/activitylog/patient/$patientProfileId/history',
    );
    if (!result.success || result.data == null) return [];
    return result.data!.map((e) => ActivityItem.fromJson(e)).toList();
  }
}
