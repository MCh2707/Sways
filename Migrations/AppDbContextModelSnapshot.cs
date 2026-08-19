using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sways.Data;

#nullable disable

namespace Sways.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("Sways.Models.BlogPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastEdited")
                        .HasColumnType("TEXT");

                    b.Property<string>("MediaFilePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("MediaInfo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Visibility")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("BlogPosts");
                });

            modelBuilder.Entity("Sways.Models.Club", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("IconUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("InviteCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("InviteCode")
                        .IsUnique();

                    b.ToTable("Clubs");
                });

            modelBuilder.Entity("Sways.Models.ClubMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ClubId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("JoinedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.HasIndex("UserId");

                    b.ToTable("ClubMembers");
                });

            modelBuilder.Entity("Sways.Models.DirectMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsRead")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ReceiverId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SenderId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("DirectMessages");
                });

            modelBuilder.Entity("Sways.Models.PollOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("PollPostId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PollPostId");

                    b.ToTable("PollOptions");
                });

            modelBuilder.Entity("Sways.Models.PollPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsSingleChoice")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("PollPosts");
                });

            modelBuilder.Entity("Sways.Models.PollVote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("PollOptionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PollPostId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("VotedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PollOptionId");

                    b.HasIndex("PollPostId");

                    b.HasIndex("UserId");

                    b.ToTable("PollVotes");
                });

            modelBuilder.Entity("Sways.Models.PostComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BlogPostId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("PollPostId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BlogPostId");

                    b.HasIndex("PollPostId");

                    b.HasIndex("UserId");

                    b.ToTable("PostComments");
                });

            modelBuilder.Entity("Sways.Models.PostLike", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BlogPostId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LikedAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("PollPostId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BlogPostId");

                    b.HasIndex("PollPostId");

                    b.HasIndex("UserId");

                    b.ToTable("PostLikes");
                });

            modelBuilder.Entity("Sways.Models.UserAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AboutMe")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("BannerUrl")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("DiscordLink")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("InstagramLink")
                        .HasColumnType("TEXT");

                    b.Property<string>("MailLink")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfilePicUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("SpotifyLink")
                        .HasColumnType("TEXT");

                    b.Property<string>("TikTokLink")
                        .HasColumnType("TEXT");

                    b.Property<string>("TwitterLink")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("UserAccounts");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AboutMe = "This is a simple about section for the user.",
                            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            DiscordLink = "#",
                            Email = "demo@sways.com",
                            Gender = "she/her",
                            InstagramLink = "#",
                            MailLink = "#",
                            Name = "Demo User",
                            PasswordHash = "password123",
                            SpotifyLink = "#",
                            TikTokLink = "#",
                            TwitterLink = "#",
                            Username = "username"
                        });
                });

            modelBuilder.Entity("Sways.Models.UserFollower", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FollowedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("FollowerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FollowingId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FollowerId");

                    b.HasIndex("FollowingId");

                    b.ToTable("UserFollowers");
                });

            modelBuilder.Entity("Sways.Models.BlogPost", b =>
                {
                    b.HasOne("Sways.Models.UserAccount", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Sways.Models.Club", b =>
                {
                    b.HasOne("Sways.Models.UserAccount", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedByUser");
                });

            modelBuilder.Entity("Sways.Models.ClubMember", b =>
                {
                    b.HasOne("Sways.Models.Club", "Club")
                        .WithMany("Members")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sways.Models.UserAccount", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Club");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Sways.Models.DirectMessage", b =>
                {
                    b.HasOne("Sways.Models.UserAccount", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Sways.Models.UserAccount", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Sways.Models.PollOption", b =>
                {
                    b.HasOne("Sways.Models.PollPost", "PollPost")
                        .WithMany("Options")
                        .HasForeignKey("PollPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PollPost");
                });

            modelBuilder.Entity("Sways.Models.PollPost", b =>
                {
                    b.HasOne("Sways.Models.UserAccount", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Sways.Models.PollVote", b =>
                {
                    b.HasOne("Sways.Models.PollOption", "PollOption")
                        .WithMany()
                        .HasForeignKey("PollOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sways.Models.PollPost", "PollPost")
                        .WithMany("Votes")
                        .HasForeignKey("PollPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sways.Models.UserAccount", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PollOption");

                    b.Navigation("PollPost");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Sways.Models.PostComment", b =>
                {
                    b.HasOne("Sways.Models.BlogPost", "BlogPost")
                        .WithMany("Comments")
                        .HasForeignKey("BlogPostId");

                    b.HasOne("Sways.Models.PollPost", "PollPost")
                        .WithMany("Comments")
                        .HasForeignKey("PollPostId");

                    b.HasOne("Sways.Models.UserAccount", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BlogPost");

                    b.Navigation("PollPost");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Sways.Models.PostLike", b =>
                {
                    b.HasOne("Sways.Models.BlogPost", "BlogPost")
                        .WithMany("Likes")
                        .HasForeignKey("BlogPostId");

                    b.HasOne("Sways.Models.PollPost", "PollPost")
                        .WithMany("Likes")
                        .HasForeignKey("PollPostId");

                    b.HasOne("Sways.Models.UserAccount", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BlogPost");

                    b.Navigation("PollPost");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Sways.Models.UserFollower", b =>
                {
                    b.HasOne("Sways.Models.UserAccount", "Follower")
                        .WithMany()
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Sways.Models.UserAccount", "Following")
                        .WithMany()
                        .HasForeignKey("FollowingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Follower");

                    b.Navigation("Following");
                });

            modelBuilder.Entity("Sways.Models.BlogPost", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");
                });

            modelBuilder.Entity("Sways.Models.Club", b =>
                {
                    b.Navigation("Members");
                });

            modelBuilder.Entity("Sways.Models.PollPost", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");

                    b.Navigation("Options");

                    b.Navigation("Votes");
                });
#pragma warning restore 612, 618
        }
    }
}
