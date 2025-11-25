import 'package:flutter/material.dart';
import 'app_colors.dart';
import 'app_text_styles.dart';

class AppTheme {
  static ThemeData get lightTheme {
    return ThemeData(
      scaffoldBackgroundColor: AppColors.background,
      primaryColor: AppColors.primary,
      textTheme: const TextTheme(
        headlineMedium: AppTextStyles.headline,
        bodyMedium: AppTextStyles.body,
      ),
      useMaterial3: true,
    );
  }
}
