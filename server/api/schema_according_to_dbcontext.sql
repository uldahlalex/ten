CREATE TABLE "users" (
    "user_id" TEXT NOT NULL CONSTRAINT "users_pkey" PRIMARY KEY,
    "email" TEXT NOT NULL,
    "salt" TEXT NULL,
    "password_hash" TEXT NULL,
    "role" TEXT NOT NULL,
    "created_at" TEXT NOT NULL,
    "totp_secret" TEXT NULL
);


CREATE TABLE "tags" (
    "tag_id" TEXT NOT NULL CONSTRAINT "tags_pkey" PRIMARY KEY,
    "name" TEXT NOT NULL,
    "user_id" TEXT NOT NULL,
    "created_at" TEXT NOT NULL,
    CONSTRAINT "tags_user_id_fkey" FOREIGN KEY ("user_id") REFERENCES "users" ("user_id")
);


CREATE TABLE "tasklist" (
    "list_id" TEXT NOT NULL CONSTRAINT "tasklist_pkey" PRIMARY KEY,
    "user_id" TEXT NOT NULL,
    "name" TEXT NOT NULL,
    "created_at" TEXT NOT NULL,
    CONSTRAINT "tasklist_user_id_fkey" FOREIGN KEY ("user_id") REFERENCES "users" ("user_id")
);


CREATE TABLE "tickticktask" (
    "task_id" TEXT NOT NULL CONSTRAINT "tickticktask_pkey" PRIMARY KEY,
    "list_id" TEXT NOT NULL,
    "title" TEXT NOT NULL,
    "description" TEXT NOT NULL,
    "due_date" TEXT NULL,
    "priority" INTEGER NOT NULL,
    "completed" INTEGER NOT NULL,
    "created_at" TEXT NOT NULL,
    "completed_at" TEXT NULL,
    CONSTRAINT "tickticktask_list_id_fkey" FOREIGN KEY ("list_id") REFERENCES "tasklist" ("list_id")
);


CREATE TABLE "task_tags" (
    "task_id" TEXT NOT NULL,
    "tag_id" TEXT NOT NULL,
    "created_at" TEXT NOT NULL,
    CONSTRAINT "task_tags_pkey" PRIMARY KEY ("task_id", "tag_id"),
    CONSTRAINT "task_tags_tag_id_fkey" FOREIGN KEY ("tag_id") REFERENCES "tags" ("tag_id"),
    CONSTRAINT "task_tags_task_id_fkey" FOREIGN KEY ("task_id") REFERENCES "tickticktask" ("task_id")
);


CREATE INDEX "idx_tags_user_id" ON "tags" ("user_id");


CREATE INDEX "IX_task_tags_tag_id" ON "task_tags" ("tag_id");


CREATE INDEX "idx_lists_user_id" ON "tasklist" ("user_id");


CREATE INDEX "idx_tasks_due_date" ON "tickticktask" ("due_date");


CREATE INDEX "idx_tasks_list_id" ON "tickticktask" ("list_id");


