# Octagram (Instagram Clone) API

## Introduction

Octagram is a social media platform inspired by Instagram, built using ASP.NET Core 8 Web API. This documentation provides a detailed overview of the project's architecture, API endpoints, data models, and implementation details.

## Project Structure

The project is structured in a classic layered architecture:

- **Octagram.API**: Contains the ASP.NET Core Web API project, responsible for handling HTTP requests and responses.
- **Octagram.Application**: Holds the application logic, including services, interfaces, and DTOs.
- **Octagram.Domain**: Defines the core business entities and domain logic.
- **Octagram.Infrastructure**: Implements data access, repositories, and other infrastructure concerns.

## Data Model

The following entities make up the core data model of Octagram:

- **User**: Represents a user of the platform. Includes attributes like `Username`, `Email`, `PasswordHash`, `Bio`, and `ProfileImageUrl`.
- **Post**: A post created by a user, containing an `ImageUrl`, `Caption`, and related `Likes`, `Comments`, and `Hashtags`.
- **Like**: Represents a user liking a post.
- **Comment**: A comment on a post, created by a user.
- **Follow**: Tracks follower/following relationships between users.
- **Story**: A time-limited visual content shared by users.
- **Hashtag**: Used to categorize posts, allowing for searching and discovery.
- **PostHashtag**: Links posts and hashtags in a many-to-many relationship.
- **DirectMessage**: Represents a private message between two users.
- **Notification**: Stores notifications for users, such as likes, comments, follows, or messages.

## API Endpoints

The API endpoints are grouped by functionality:

### 1. Authentication

- `POST /api/authentication/register`: Registers a new user.
- `POST /api/authentication/login`: Authenticates a user and returns a JWT token.
- `POST /api/authentication/refresh`: Refreshes a user's expired JWT token.

### 2. Users

- `GET /api/users/{userId}`: Retrieves a user profile by ID.
- `GET /api/users/username/{username}`: Retrieves a user profile by username.
- `PUT /api/users/`: Updates current user's profile information.
- `GET /api/users/search/{query}`: Searches for users based on a query string.
- `POST /api/users/follow/{followingId}`: Follows another user.
- `DELETE /api/users/follow/{followingId}`: Unfollows another user.
- `GET /api/users/{userId}/followers`: Retrieves a list of a user's followers.
- `GET /api/users/{userId}/following`: Retrieves a list of users a user is following.

### 3. Posts

- `GET /api/posts`: Retrieves all posts, with optional pagination and hashtag filtering.
- `GET /api/posts/feed`: Gets posts from followed users for the current user, with optional pagination.
- `GET /api/posts/{postId}`: Retrieves a specific post by ID.
- `GET /api/posts/user/{userId}`: Retrieves posts by a specific user.
- `POST /api/posts`: Creates a new post.
- `PUT /api/posts/{postId}`: Updates a post's caption.
- `DELETE /api/posts/{postId}`: Deletes a post.
- `POST /api/posts/{postId}/like`: Likes a post.
- `DELETE /api/posts/{postId}/like`: Unlikes a post.
- `POST /api/posts/{postId}/comments`: Adds a comment to a post.
- `DELETE /api/posts/{postId}/comments/{commentId}`: Deletes a comment.

### 4. Stories

- `GET /api/stories/user/{userId}`: Retrieves stories for a specific user.
- `GET /api/stories/following`: Retrieves stories from followed users.
- `POST /api/stories`: Creates a new story (image or video).
- `DELETE /api/stories/{storyId}`: Deletes a story.

### 5. Search

- `GET /api/search/users/{query}`: Searches for users.
- `GET /api/search/posts/{query}`: Searches for posts.
- `GET /api/search/hashtags/{query}`: Searches for hashtags.

### 6. Direct Messages

- `GET /api/directmessage/{targetUserId}`: Retrieves conversation between current user and target user, with pagination.
- `POST /api/directmessage`: Sends a direct message (for testing, see `DirectMessagesHub` for real-time functionality).
- `PATCH /api/directmessage/{messageId}/read`: Marks a message as read.

### 7. Notifications

- `GET /api/notifications`: Retrieves notifications for the authenticated user, with pagination.
- `PATCH /api/notifications/{notificationId}/read`: Marks a notification as read.

### 8. SignalR Hubs

- **DirectMessageHub**: Handles real-time direct message communication between users.
- **NotificationHub**: Provides real-time notification updates to clients.

## Key Technologies and Concepts

- **ASP.NET Core 8 Web API**: Framework for building RESTful APIs.
- **Entity Framework Core**: ORM for database interaction.
- **AutoMapper**: Object-object mapping library used for DTO mapping.
- **JWT (JSON Web Tokens)**: Authentication and authorization mechanism.
- **SignalR**: Real-time communication framework for instant updates.
- **Cloud Storage**: Azure Blob Storage is used for storing media files (images, videos).

## Authentication and Authorization

- Authentication is handled using JWT tokens.
- The `AuthorizeMiddlewareAttribute` custom attribute is used to enforce role-based authorization on specific endpoints.

## Image Processing and Storage

- The `IImageHelper` interface and `ImageHelper` class handle image resizing and optimization.
- The `ICloudStorageHelper` interface and `CloudStorageHelper` class manage file uploads to Azure Blob Storage.

## Real-time Communication

- The `DirectMessageHub` and `NotificationHub` SignalR hubs facilitate real-time communication for messages and notifications, respectively.

## Data Seeding

- The `DataSeeder` class is responsible for seeding the database with sample data for testing purposes.

## Further Development

- Implement video upload and processing for stories.
- Add additional social features (e.g., sharing, reposting).
- Improve error handling and logging.

## License

This project is licensed under the MIT License.