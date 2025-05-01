using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using efscaffold.Entities;

namespace Infrastructure.Postgres.Scaffolding;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TaskTag> TaskTags { get; set; }

    public virtual DbSet<Tasklist> Tasklists { get; set; }

    public virtual DbSet<Tickticktask> Tickticktasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("tags_pkey");

            entity.ToTable("tags", "ticktick");

            entity.HasIndex(e => e.UserId, "idx_tags_user_id");

            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Tags)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tags_user_id_fkey");
        });

        modelBuilder.Entity<TaskTag>(entity =>
        {
            entity.HasKey(e => new { e.TaskId, e.TagId }).HasName("task_tags_pkey");

            entity.ToTable("task_tags", "ticktick");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(d => d.Tag).WithMany(p => p.TaskTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("task_tags_tag_id_fkey");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskTags)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("task_tags_task_id_fkey");
        });

        modelBuilder.Entity<Tasklist>(entity =>
        {
            entity.HasKey(e => e.ListId).HasName("tasklist_pkey");

            entity.ToTable("tasklist", "ticktick");

            entity.HasIndex(e => e.UserId, "idx_lists_user_id");

            entity.Property(e => e.ListId).HasColumnName("list_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Tasklists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tasklist_user_id_fkey");
        });

        modelBuilder.Entity<Tickticktask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("tickticktask_pkey");

            entity.ToTable("tickticktask", "ticktick");

            entity.HasIndex(e => e.DueDate, "idx_tasks_due_date");

            entity.HasIndex(e => e.ListId, "idx_tasks_list_id");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.Completed).HasColumnName("completed");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.ListId).HasColumnName("list_id");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.List).WithMany(p => p.Tickticktasks)
                .HasForeignKey(d => d.ListId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickticktask_list_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users", "ticktick");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Salt).HasColumnName("salt");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
