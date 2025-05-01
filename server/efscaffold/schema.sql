drop schema if exists ticktick cascade;
create schema if not exists ticktick;

CREATE TABLE ticktick.users
(
    user_id       TEXT PRIMARY KEY,
    email         TEXT        NOT NULL,
    salt          TEXT        NOT NULL,
    password_hash TEXT        NOT NULL,
    role          TEXT        NOT NULL,
    created_at    TIMESTAMPTZ NOT NULL
);

CREATE TABLE ticktick.tasklist
(
    list_id    TEXT PRIMARY KEY,
    user_id    TEXT        NOT NULL,
    name       TEXT        NOT NULL,
    created_at TIMESTAMPTZ NOT NULL,
    FOREIGN KEY (user_id) REFERENCES ticktick.users (user_id)
);

CREATE TABLE ticktick.tickticktask
(
    task_id      TEXT PRIMARY KEY,
    list_id      TEXT        NOT NULL,
    title        TEXT        NOT NULL,
    description  TEXT        NOT NULL,
    due_date     TIMESTAMPTZ DEFAULT NULL,
    priority     INTEGER     NOT NULL,
    completed    BOOLEAN     NOT NULL,
    created_at   TIMESTAMPTZ NOT NULL,
    completed_at TIMESTAMPTZ DEFAULT NULL,
    FOREIGN KEY (list_id) REFERENCES ticktick.tasklist (list_id)
);

CREATE TABLE ticktick.tags
(
    tag_id     TEXT PRIMARY KEY,
    name       TEXT        NOT NULL,
    user_id    TEXT        NOT NULL,
    created_at TIMESTAMPTZ NOT NULL,
    FOREIGN KEY (user_id) REFERENCES ticktick.users (user_id)
);

CREATE TABLE ticktick.task_tags
(
    task_id    TEXT        NOT NULL,
    tag_id     TEXT        NOT NULL,
    created_at TIMESTAMPTZ NOT NULL,
    PRIMARY KEY (task_id, tag_id),
    FOREIGN KEY (task_id) REFERENCES ticktick.tickticktask (task_id),
    FOREIGN KEY (tag_id) REFERENCES ticktick.tags (tag_id)
);

-- Create indexes for better query performance
CREATE INDEX idx_tasks_list_id ON ticktick.tickticktask (list_id);
CREATE INDEX idx_tasks_due_date ON ticktick.tickticktask (due_date);
CREATE INDEX idx_lists_user_id ON ticktick.tasklist (user_id);
CREATE INDEX idx_tags_user_id ON ticktick.tags (user_id);
