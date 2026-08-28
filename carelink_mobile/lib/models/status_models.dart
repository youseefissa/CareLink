class AlertItem {
  final String id;
  final String message;
  final int severity;
  final bool isResolved;
  final DateTime createdAt;

  AlertItem({
    required this.id,
    required this.message,
    required this.severity,
    required this.isResolved,
    required this.createdAt,
  });

  factory AlertItem.fromJson(Map<String, dynamic> json) {
    return AlertItem(
      id: json['id'],
      message: json['message'],
      severity: json['severity'],
      isResolved: json['isResolved'],
      createdAt: DateTime.parse(json['createdAt']),
    );
  }
}

class RecommendationItem {
  final String id;
  final String recommendationText;
  final String category;
  final bool isAcknowledged;

  RecommendationItem({
    required this.id,
    required this.recommendationText,
    required this.category,
    required this.isAcknowledged,
  });

  factory RecommendationItem.fromJson(Map<String, dynamic> json) {
    return RecommendationItem(
      id: json['id'],
      recommendationText: json['recommendationText'],
      category: json['category'],
      isAcknowledged: json['isAcknowledged'],
    );
  }
}

class ActivityItem {
  final String activityType;
  final String? details;
  final DateTime occurredAt;

  ActivityItem({
    required this.activityType,
    this.details,
    required this.occurredAt,
  });

  factory ActivityItem.fromJson(Map<String, dynamic> json) {
    return ActivityItem(
      activityType: json['activityType'],
      details: json['details'],
      occurredAt: DateTime.parse(json['occurredAt']),
    );
  }
}
