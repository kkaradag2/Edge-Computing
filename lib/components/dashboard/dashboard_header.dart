import 'package:flutter/material.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_text_styles.dart';

class DashboardHeader extends StatelessWidget {
  const DashboardHeader({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(AppSpacing.lg),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(24),
        gradient: const LinearGradient(
          colors: [Color(0xFFF7F7F7), Color(0xFFEFEFEF)],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: 12,
            offset: Offset(0, 6),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Avatar + Greeting + Notification
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Row(
                children: [
                  ClipRRect(
                    borderRadius: BorderRadius.circular(50),
                    child: Image.asset(
                      'assets/images/profile.jpg',
                      width: 48,
                      height: 48,
                      fit: BoxFit.cover,
                    ),
                  ),
                  const SizedBox(width: AppSpacing.md),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text("Good Morning,", style: AppTextStyles.body),
                      Text(
                        "Leslie Smith!",
                        style: AppTextStyles.title.copyWith(
                          color: AppColors.textDark,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
              Icon(
                Icons.notifications_none,
                size: 26,
                color: AppColors.textDark,
              ),
            ],
          ),

          const SizedBox(height: AppSpacing.lg),

          // Insight card content
          Text(
            "You’ve focused on protein this week 💪",
            style: AppTextStyles.title,
          ),
          const SizedBox(height: AppSpacing.sm),
          Text(
            "Maybe add some vegetables today for balance 🍀",
            style: AppTextStyles.body.copyWith(color: AppColors.textLight),
          ),
        ],
      ),
    );
  }
}
