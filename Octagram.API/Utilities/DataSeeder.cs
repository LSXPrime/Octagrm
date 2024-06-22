using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.API.Utilities;

/// <summary>
/// Class responsible for seeding the database with sample data.
/// </summary>
/// <param name="context">The database context.</param>
public class DataSeeder(ApplicationDbContext context)
{
    private readonly Random _random = new();

    // Sample data for posts and comments
    private readonly List<string> _imageUrls =
    [
        "https://picsum.photos/500/500",
        "https://picsum.photos/600/400",
        "https://picsum.photos/700/500",
        "https://picsum.photos/800/650",
        "https://picsum.photos/900/700"
    ];
    
    private readonly List<string> _storyImageUrls =
    [
        "https://picsum.photos/600/800",
        "https://picsum.photos/800/600",
        "https://picsum.photos/700/900",
        "https://picsum.photos/900/850",
        "https://picsum.photos/1000/700"
    ];

    private readonly List<string> _captions =
    [
        "Having a blast! #fun #friends",
        "Enjoying the view #travel #nature",
        "Foodie for life #food #delicious",
        "Back to reality #work #grind",
        "Weekend vibes #relax #chill"
    ];

    private readonly List<string> _commentsContent =
    [
        "Awesome!",
        "Looks great!",
        "Where is this?",
        "Can't wait to see more!",
        "Amazing photo!"
    ];

    private readonly List<string> _messageContent =
    [
        "Hey, what's up?",
        "How are you doing?",
        "Did you see that post?",
        "Let's hang out sometime!",
        "I'm working on this cool project."
    ];

    private readonly List<string> _notificationTypes =
    [
        "like",
        "comment",
        "follow"
    ];

    /// <summary>
    /// Seeds the database with sample data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SeedAsync()
    {
        await SeedUsersAsync();
        await SeedPostsAsync();
        await SeedLikesAsync();
        await SeedCommentsAsync();
        await SeedFollowsAsync();
        await SeedDirectMessagesAsync();
        await SeedNotificationsAsync();
        await SeedStoriesAsync();
        await SeedHashtagsAndPostHashtags();
    }

    /// <summary>
    /// Seeds the database with sample user data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SeedUsersAsync()
    {
        if (!await context.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new()
                {
                    Username = "johndoe", Email = "johndoe@example.com", PasswordHash = HashPassword("password"),
                    ProfileImageUrl = "https://picsum.photos/200/200",
                    Bio = "I am a software developer and love to travel."
                },
                new()
                {
                    Username = "janedoe", Email = "janedoe@example.com", PasswordHash = HashPassword("password"),
                    ProfileImageUrl = "https://picsum.photos/200/200",
                    Bio = "I am a photographer and love to travel."
                },
                new()
                {
                    Username = "alice", Email = "alice@example.com", PasswordHash = HashPassword("password"),
                    ProfileImageUrl = "https://picsum.photos/200/200",
                    Bio = "I am a musician and love to travel."
                },
                new()
                {
                    Username = "bob", Email = "bob@example.com", PasswordHash = HashPassword("password"),
                    ProfileImageUrl = "https://picsum.photos/200/200",
                    Bio = "I am cat girl, meow"
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Seeds the database with sample post data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SeedPostsAsync()
    {
        if (!await context.Posts.AnyAsync())
        {
            var users = await context.Users.ToListAsync();
            foreach (var user in users)
            {
                var postCount = _random.Next(3, 6); // Each user creates 3-5 posts
                for (int i = 0; i < postCount; i++)
                {
                    var post = new Post
                    {
                        UserId = user.Id,
                        ImageUrl = _imageUrls[_random.Next(_imageUrls.Count)],
                        Caption = _captions[_random.Next(_captions.Count)],
                        CreatedAt = DateTime.UtcNow.AddDays(_random.Next(-30, 0))
                    };
                    await context.Posts.AddAsync(post);
                }
            }

            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Seeds the database with sample post's like data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SeedLikesAsync()
    {
        if (!await context.Likes.AnyAsync())
        {
            var posts = await context.Posts.ToListAsync();
            var users = await context.Users.ToListAsync();

            foreach (var post in posts)
            {
                // Randomly select users to like the post
                var usersToLikePost = users.OrderBy(_ => Guid.NewGuid()).Take(_random.Next(1, users.Count)).ToList();

                foreach (var user in usersToLikePost)
                {
                    var like = new Like { UserId = user.Id, PostId = post.Id };
                    await context.Likes.AddAsync(like);
                }
            }

            await context.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Seeds the database with sample post's comment data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SeedCommentsAsync()
    {
        if (!await context.Comments.AnyAsync())
        {
            var posts = await context.Posts.ToListAsync();
            var users = await context.Users.ToListAsync();
            foreach (var post in posts)
            {
                var commentCount = _random.Next(1, 4); // 1-3 comments per post
                for (int i = 0; i < commentCount; i++)
                {
                    var comment = new Comment
                    {
                        UserId = users[_random.Next(users.Count)].Id, // Random user comments
                        PostId = post.Id,
                        Content = _commentsContent[_random.Next(_commentsContent.Count)],
                        CreatedAt = DateTime.UtcNow.AddDays(
                            _random.Next((int)(post.CreatedAt - DateTime.UtcNow).TotalDays, 0))
                        // Comment created after the post, within the post's timeframe
                    };
                    await context.Comments.AddAsync(comment);
                }
            }

            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Seeds the database with sample user follow data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SeedFollowsAsync()
    {
        if (!await context.Follows.AnyAsync())
        {
            var users = await context.Users.ToListAsync();
            foreach (var user in users)
            {
                // Ensure a user doesn't follow themselves
                var usersToFollow = users.Where(u => u.Id != user.Id)
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(_random.Next(1, users.Count - 1)) // Follow 1 to all but one other user
                    .ToList();

                foreach (var userToFollow in usersToFollow)
                {
                    var follow = new Follow { FollowerId = user.Id, FollowingId = userToFollow.Id };
                    await context.Follows.AddAsync(follow);
                }
            }

            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Seeds the database with sample users direct message data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SeedDirectMessagesAsync()
    {
        if (!await context.DirectMessages.AnyAsync())
        {
            var users = await context.Users.ToListAsync();
            for (var i = 0; i < users.Count - 1; i++) // Create conversations between users
            {
                for (int j = i + 1; j < users.Count; j++)
                {
                    var sender = users[i];
                    var receiver = users[j];
                    var messageCount = _random.Next(1, 5); // 1-4 messages per conversation

                    for (int k = 0; k < messageCount; k++)
                    {
                        var message = new DirectMessage
                        {
                            SenderId = sender.Id,
                            ReceiverId = receiver.Id,
                            Content = _messageContent[_random.Next(_messageContent.Count)],
                            CreatedAt = DateTime.UtcNow.AddDays(_random.Next(-14,
                                0)), // Messages within the past 2 weeks
                            IsRead = _random.Next(2) == 0 // Randomly mark some messages as read
                        };
                        await context.DirectMessages.AddAsync(message);
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Seeds the database with sample notification data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SeedNotificationsAsync()
    {
        if (!await context.Notifications.AnyAsync())
        {
            var users = await context.Users.ToListAsync();
            var posts = await context.Posts.ToListAsync();

            foreach (var user in users)
            {
                var notificationCount = _random.Next(1, 6); // 1-5 notifications per user
                for (int i = 0; i < notificationCount; i++)
                {
                    var notification = new Notification
                    {
                        RecipientId = user.Id,
                        SenderId = users[_random.Next(users.Count)].Id, // Random sender
                        Type = _notificationTypes[_random.Next(_notificationTypes.Count)],
                        TargetId = posts[_random.Next(posts.Count)].Id, // Randomly link to a post
                        CreatedAt = DateTime.UtcNow.AddDays(_random.Next(-7, 0)), // Notifications within the past week
                        IsRead = _random.Next(2) == 0 // Randomly mark some as read
                    };
                    await context.Notifications.AddAsync(notification);
                }
            }

            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Seeds the database with sample story data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SeedStoriesAsync()
    {
        if (!await context.Stories.AnyAsync())
        {
            var users = await context.Users.ToListAsync();
            foreach (var user in users)
            {
                var storyCount = _random.Next(1, 4); // 1-3 stories per user
                for (int i = 0; i < storyCount; i++)
                {
                    var story = new Story
                    {
                        UserId = user.Id,
                        MediaUrl = _storyImageUrls[_random.Next(_storyImageUrls.Count)],
                        MediaType = "image", 
                        CreatedAt = DateTime.UtcNow.AddHours(_random.Next(-12, 0)), // Stories within last 12 hours
                        ExpiresAt = DateTime.UtcNow.AddHours(_random.Next(12, 24)) // Expire in 12-24 hours
                    };
                    await context.Stories.AddAsync(story);
                }
            }

            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Seeds the database with sample hashtag and post hashtag data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SeedHashtagsAndPostHashtags()
    {
        if (await context.Hashtags.AnyAsync())
        {
            return;
        }

        var posts = await context.Posts.ToListAsync();

        foreach (var post in posts)
        {
            var hashtagsInCaption = Regex.Matches(post.Caption!, @"#(\w+)")
                .Select(m => m.Groups[1].Value.ToLowerInvariant())
                .Distinct()
                .ToList();

            foreach (var hashtagName in hashtagsInCaption)
            {
                var hashtag = await context.Hashtags.FirstOrDefaultAsync(h => h.Name == hashtagName) ??
                              new Hashtag { Name = hashtagName };

                if (context.Entry(hashtag).State == EntityState.Detached)
                {
                    await context.Hashtags.AddAsync(hashtag);
                    await context.SaveChangesAsync();
                }

                var postHashtag = new PostHashtag { PostId = post.Id, HashtagId = hashtag.Id };
                await context.PostHashtags.AddAsync(postHashtag);
            }
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Hashes a password using PBKDF2 with a hardcoded salt.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password.</returns>
    private string HashPassword(string password)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: "SOME_RANDOM_SALT_VALUE"u8.ToArray(),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
    }
}