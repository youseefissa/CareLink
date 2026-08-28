import 'package:flutter/material.dart';
import '../theme/app_theme.dart';
import '../models/status_models.dart';
import '../services/status_service.dart';
import '../services/token_storage_service.dart';

class StatusScreen extends StatefulWidget {
  const StatusScreen({super.key});

  @override
  State<StatusScreen> createState() => _StatusScreenState();
}

class _StatusScreenState extends State<StatusScreen> {
  final _statusService = StatusService();
  final _tokenStorage = TokenStorageService();

  List<AlertItem> _alerts = [];
  List<RecommendationItem> _recommendations = [];
  List<ActivityItem> _activityLogs = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() {
      _isLoading = true;
    });

    final patientProfileId = await _tokenStorage.getPatientProfileId();

    if (patientProfileId == null) {
      setState(() {
        _isLoading = false;
      });
      return;
    }

    final results = await Future.wait([
      _statusService.getAlerts(patientProfileId),
      _statusService.getRecommendations(patientProfileId),
      _statusService.getActivityHistory(patientProfileId),
    ]);

    if (!mounted) return;

    setState(() {
      _alerts = results[0] as List<AlertItem>;
      _recommendations = results[1] as List<RecommendationItem>;
      _activityLogs = results[2] as List<ActivityItem>;
      _isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    final unresolvedAlerts = _alerts.where((a) => !a.isResolved).toList();
    final pendingRecommendations = _recommendations
        .where((r) => !r.isAcknowledged)
        .toList();
    final lastActivity = _activityLogs.isNotEmpty ? _activityLogs.first : null;

    return Scaffold(
      appBar: AppBar(
        title: const Text('حالتي'),
        backgroundColor: AppColors.ink,
        foregroundColor: Colors.white,
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
              onRefresh: _loadData,
              child: ListView(
                padding: const EdgeInsets.all(20),
                children: [
                  _buildLastActivityCard(lastActivity),
                  const SizedBox(height: 20),
                  _buildSectionTitle('التنبيهات غير المحلولة'),
                  const SizedBox(height: 10),
                  if (unresolvedAlerts.isEmpty)
                    _buildEmptyText('لا توجد تنبيهات مفتوحة حاليًا')
                  else
                    ...unresolvedAlerts.map(_buildAlertCard),
                  const SizedBox(height: 24),
                  _buildSectionTitle('التوصيات الوقائية'),
                  const SizedBox(height: 10),
                  if (pendingRecommendations.isEmpty)
                    _buildEmptyText('لا توجد توصيات جديدة')
                  else
                    ...pendingRecommendations.map(_buildRecommendationCard),
                ],
              ),
            ),
    );
  }

  Widget _buildLastActivityCard(ActivityItem? lastActivity) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.border),
      ),
      child: Row(
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: const BoxDecoration(
              color: AppColors.calm,
              shape: BoxShape.circle,
            ),
            child: const Icon(Icons.directions_walk, color: Colors.white),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'آخر نشاط مسجل',
                  style: TextStyle(fontSize: 13, color: Colors.black54),
                ),
                const SizedBox(height: 4),
                Text(
                  lastActivity != null
                      ? _formatDate(lastActivity.occurredAt)
                      : 'لا يوجد نشاط مسجل بعد',
                  style: const TextStyle(
                    fontSize: 15,
                    fontWeight: FontWeight.w600,
                    color: AppColors.ink,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSectionTitle(String title) {
    return Text(
      title,
      style: const TextStyle(
        fontSize: 17,
        fontWeight: FontWeight.bold,
        color: AppColors.ink,
      ),
    );
  }

  Widget _buildEmptyText(String text) {
    return Text(
      text,
      style: const TextStyle(fontSize: 13, color: Colors.black45),
    );
  }

  Widget _buildAlertCard(AlertItem alert) {
    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppColors.critical.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(10),
        border: Border(right: BorderSide(color: AppColors.critical, width: 3)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            alert.message,
            style: const TextStyle(fontSize: 14, color: AppColors.ink),
          ),
          const SizedBox(height: 4),
          Text(
            _formatDate(alert.createdAt),
            style: const TextStyle(fontSize: 11, color: Colors.black45),
          ),
        ],
      ),
    );
  }

  Widget _buildRecommendationCard(RecommendationItem rec) {
    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppColors.calm.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(10),
        border: Border(right: BorderSide(color: AppColors.calm, width: 3)),
      ),
      child: Text(
        rec.recommendationText,
        style: const TextStyle(fontSize: 14, color: AppColors.ink),
      ),
    );
  }

  String _formatDate(DateTime date) {
    final local = date.toLocal();
    return '${local.day}/${local.month}/${local.year} - ${local.hour.toString().padLeft(2, '0')}:${local.minute.toString().padLeft(2, '0')}';
  }
}
