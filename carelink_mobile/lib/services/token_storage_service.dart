import 'package:shared_preferences/shared_preferences.dart';

class TokenStorageService {
  static const _tokenKey = 'carelink_token';
  static const _refreshTokenKey = 'carelink_refresh_token';
  static const _userIdKey = 'carelink_user_id';
  static const _patientProfileIdKey = 'carelink_patient_profile_id';

  Future<void> saveSession({
    required String token,
    required String refreshToken,
    required String userId,
  }) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_tokenKey, token);
    await prefs.setString(_refreshTokenKey, refreshToken);
    await prefs.setString(_userIdKey, userId);
  }

  Future<void> savePatientProfileId(String patientProfileId) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_patientProfileIdKey, patientProfileId);
  }

  Future<String?> getPatientProfileId() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_patientProfileIdKey);
  }

  Future<String?> getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_tokenKey);
  }

  Future<String?> getUserId() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_userIdKey);
  }

  Future<void> clearSession() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_tokenKey);
    await prefs.remove(_refreshTokenKey);
    await prefs.remove(_userIdKey);
    await prefs.remove(_patientProfileIdKey);
  }
}
