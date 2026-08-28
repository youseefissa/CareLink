import '../models/auth_models.dart';
import 'api_client.dart';
import 'token_storage_service.dart';

class AuthService {
  final ApiClient _apiClient = ApiClient();
  final TokenStorageService _tokenStorage = TokenStorageService();

  Future<ApiResult<AuthResponse>> login(String email, String password) async {
    final result = await _apiClient.post(
      '/api/auth/login',
      LoginRequest(email: email, password: password).toJson(),
      withAuth: false,
    );

    if (!result.success || result.data == null) {
      return ApiResult(success: false, errors: result.errors);
    }

    final authResponse = AuthResponse.fromJson(result.data!);

    await _tokenStorage.saveSession(
      token: authResponse.token,
      refreshToken: authResponse.refreshToken,
      userId: authResponse.user.id,
    );

    return ApiResult(success: true, data: authResponse);
  }
}
