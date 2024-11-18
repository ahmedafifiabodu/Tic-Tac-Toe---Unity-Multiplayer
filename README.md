# Tic Tac Toe Multiplayer Game

Welcome to the Tic Tac Toe Multiplayer Game repository! This project is a multiplayer version of the classic Tic Tac Toe game, built using the FishNet networking library. The game allows multiple players to connect and play against each other in real-time.

## Features

- **Multiplayer Support**: Play Tic Tac Toe with friends or other players online.
- **Real-Time Communication**: Instant updates and smooth gameplay experience.
- **User-Friendly Interface**: Intuitive and easy-to-use UI for seamless interaction.
- **Cross-Platform**: Compatible with various platforms supported by Unity.

## Getting Started

### Prerequisites

- **Unity**: Make sure you have Unity installed. This project was developed using Unity 2022.
- **FishNet**: The networking library used in this project. FishNet is included in the `Assets/Plugins/FishNet` directory.

### Installation

1. **Clone the Repository**:

```
git clone https://github.com/your-username/tic-tac-toe-multiplayer.git
```

2. **Open the Project in Unity**:
    - Launch Unity Hub.
    - Click on the "Add" button and select the cloned repository folder.

3. **Install Dependencies**:
    - Ensure all required packages are installed via the Unity Package Manager.

### Running the Game

1. **Start the Server**:
    - Open the `RTSGameManager` script and ensure the server settings are configured.
    - Run the Unity project and start the server from the UI.

2. **Connect Clients**:
    - Open additional instances of the game.
    - Connect to the server using the client UI.

3. **Play the Game**:
    - Once connected, players can start a new game of Tic Tac Toe and enjoy real-time multiplayer gameplay.

## Project Structure

- **Assets/Project/Script/System**: Contains the core game logic and system scripts.
  - **Chat System**: Manages the in-game chat functionality.
  - **Game Manager**: Handles the game state, player turns, and win conditions.
- **Assets/Project/Script/UI**: Contains UI-related scripts for managing the game interface.
- **Assets/Plugins/FishNet**: Includes the FishNet networking library and related utilities.

## Contributing

Contributions are welcome! If you have any ideas, suggestions, or bug reports, please open an issue or submit a pull request.

### How to Contribute

1. **Fork the Repository**:
    - Click the "Fork" button on the top right of the repository page.

2. **Create a Branch**:

```
git checkout -b feature/your-feature-name
```

3. **Commit Your Changes**:

```
git commit -m 'Add some feature'
```

4. **Push to the Branch**:

```
git push origin feature/your-feature-name
```

5. **Open a Pull Request**:
    - Go to your forked repository and click the "New Pull Request" button.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

- **FishNet**: A powerful networking library for Unity, enabling smooth and efficient multiplayer experiences.

Enjoy playing Tic Tac Toe with your friends and family! If you have any questions or need further assistance, feel free to reach out.

Happy Gaming!