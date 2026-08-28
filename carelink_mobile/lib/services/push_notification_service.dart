import 'package:firebase_messaging/firebase_messaging.dart';
import 'api_client.dart';
import 'token_storage_service.dart';

class PushNotificationService {
  final ApiClient _apiClient = ApiClient();
  final TokenStorageService _tokenStorage = TokenStorageService();

  Future<void> initializeAndRegister() async {
    final messaging = FirebaseMessaging.instance;

    await messaging.requestPermission(alert: true, badge: true, sound: true);

    final deviceToken = await messaging.getToken();

    if (deviceToken != null) {
      await _registerDeviceToken(deviceToken);
    }

    FirebaseMessaging.instance.onTokenRefresh.listen((newToken) {
      _registerDeviceToken(newToken);
    });

    FirebaseMessaging.onMessage.listen((message) {
      // الإشعار هيظهر تلقائيًا من نظام أندرويد نفسه
      // هذا فقط لأي منطق إضافي نحتاجه لاحقًا داخل التطبيق نفسه
    });
  }

  Future<void> _registerDeviceToken(String deviceToken) async {
    final userId = await _tokenStorage.getUserId();

    if (userId == null) return;

    await _apiClient.post('/api/user/device-token', {
      'deviceToken': deviceToken,
    });
  }
}
