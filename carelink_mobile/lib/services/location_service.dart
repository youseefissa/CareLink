import 'package:geolocator/geolocator.dart';

class LocationResult {
  final bool success;
  final double? latitude;
  final double? longitude;
  final String? errorMessage;

  LocationResult({
    required this.success,
    this.latitude,
    this.longitude,
    this.errorMessage,
  });
}

class LocationService {
  Future<LocationResult> getCurrentLocation() async {
    bool serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) {
      return LocationResult(
        success: false,
        errorMessage: 'خدمة الموقع مطفأة على الجهاز، من فضلك فعّلها.',
      );
    }

    LocationPermission permission = await Geolocator.checkPermission();

    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();

      if (permission == LocationPermission.denied) {
        return LocationResult(
          success: false,
          errorMessage:
              'تم رفض إذن الموقع، هذا الإذن مهم لتحديد مكانك وقت الطوارئ.',
        );
      }
    }

    if (permission == LocationPermission.deniedForever) {
      return LocationResult(
        success: false,
        errorMessage:
            'تم رفض إذن الموقع بشكل دائم، من فضلك فعّله يدويًا من إعدادات الجهاز.',
      );
    }

    try {
      final position = await Geolocator.getCurrentPosition(
        desiredAccuracy: LocationAccuracy.high,
      );

      return LocationResult(
        success: true,
        latitude: position.latitude,
        longitude: position.longitude,
      );
    } catch (e) {
      return LocationResult(
        success: false,
        errorMessage: 'تعذر الحصول على الموقع الحالي.',
      );
    }
  }
}
