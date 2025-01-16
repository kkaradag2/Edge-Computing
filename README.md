# Event-Driven Edge Node Management System Using ZooKeeper

This repository contains the source code and documentation for the **Event-Driven Edge Node Management System**, a distributed system designed to optimize edge computing by leveraging Apache ZooKeeper. The system ensures efficient node coordination, fault tolerance, and real-time task distribution in edge environments.

## Features

- **Dynamic Edge Node Management**: Handles node registration, monitoring, and task redistribution dynamically.
- **Fault Detection and Recovery**: Automatically reallocates tasks to healthy nodes upon failure detection.
- **Event-Driven Architecture**: Provides faster responses to network changes (e.g., node failures or task redistributions).
- **Scalable Design**: Maintains performance as the number of nodes grows.
- **Real-Time Monitoring**: Integrates Prometheus and Grafana for visualizing system metrics.

## Architecture

The system is built on an **Apache ZooKeeper-based architecture**, with the following components:

1. **ZooKeeper Servers**: Manage centralized coordination and node state information.
2. **Edge Nodes**: Regularly update their health status and resource utilization to ZooKeeper.
3. **Clients**: Query ZooKeeper for healthy nodes and send requests to the most appropriate one.
4. **Prometheus & Grafana**: Collect and visualize metrics such as CPU usage, RAM usage, and health status.
5. **Docker**: All components are containerized for easy deployment.

![System Architecture](/Assets/images/Topoloji.png)  


## Setup and Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/kkaradag2/Edge-Computing.git
   cd Edge-Computing
   ```
2. Start the system using Docker Compose:
   ```bash
   docker-compose up
   ```
3. Access Grafana dashboards for real-time monitoring:
   - Open a browser and navigate to `http://localhost:3000`.
   - Default credentials:
     - Username: `admin`
     - Password: `admin`

## Usage

1. **Node Registration**: Edge nodes automatically register themselves in ZooKeeper.
2. **Monitoring**: View real-time CPU and RAM usage for each node in Grafana.
3. **Client Requests**: Clients query ZooKeeper for healthy nodes and send requests accordingly.

![Node Health Monitoring](/Assets/images/Graphana.png)  


## Testing and Results

The system was tested under various scenarios to evaluate:

1. **Health Status Management**:
   - Nodes exceeding CPU (80%) or RAM (90%) thresholds were marked as unhealthy.
2. **Fault Recovery**:
   - Clients were able to bypass unhealthy nodes and access healthy ones.
3. **Real-Time Metrics**:
   - Metrics were collected every 2 seconds and visualized in Grafana.

![Test Results](/Assets/images/Terminal.png)  


## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a new branch:
   ```bash
   git checkout -b feature/your-feature
   ```
3. Commit your changes:
   ```bash
   git commit -m "Add your message"
   ```
4. Push to the branch:
   ```bash
   git push origin feature/your-feature
   ```
5. Open a pull request.

## License

This project is free to use, modify, and distribute for educational and personal purposes. It is an open resource for learning and experimentation. 
