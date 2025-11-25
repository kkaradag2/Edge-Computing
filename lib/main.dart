import 'package:flutter/material.dart';
import 'core/theme/app_theme.dart';
import 'ui/screens/dashboard/dashboard_screen.dart';

void main() {
  runApp(const BalancerApp());
}

class BalancerApp extends StatelessWidget {
  const BalancerApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      theme: AppTheme.lightTheme,
      home: const DashboardScreen(),
    );
  }
}
