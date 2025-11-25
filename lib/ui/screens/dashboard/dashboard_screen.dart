import 'package:flutter/material.dart';
import '../../../components/dashboard/dashboard_header.dart';

class DashboardScreen extends StatelessWidget {
  const DashboardScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(16),
          child: Column(children: const [DashboardHeader()]),
        ),
      ),
    );
  }
}
