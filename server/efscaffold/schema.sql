drop schema if exists ticktick cascade;
create schema if not exists ticktick;

CREATE TABLE ticktick.users (
                                user_id SERIAL PRIMARY KEY,
                                email VARCHAR(255) NOT NULL,
                                username VARCHAR(50) NOT NULL,
                                password_hash VARCHAR(255) NOT NULL,
                                created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE ticktick.lists (
                                list_id SERIAL PRIMARY KEY,
                                user_id INTEGER NOT NULL,
                                name VARCHAR(100) NOT NULL,
                                created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                FOREIGN KEY (user_id) REFERENCES ticktick.users(user_id)
);

CREATE TABLE ticktick.tickticktask (
                                task_id SERIAL PRIMARY KEY,
                                list_id INTEGER NOT NULL,
                                title VARCHAR(255) NOT NULL,
                                description TEXT NOT NULL DEFAULT '',
                                due_date TIMESTAMPTZ NOT NULL,
                                priority INTEGER NOT NULL DEFAULT 0,
                                completed BOOLEAN NOT NULL DEFAULT FALSE,
                                created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                completed_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                FOREIGN KEY (list_id) REFERENCES ticktick.lists(list_id)
);

CREATE TABLE ticktick.tags (
                               tag_id SERIAL PRIMARY KEY,
                               name VARCHAR(50) NOT NULL,
                               user_id INTEGER NOT NULL,
                               created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                               FOREIGN KEY (user_id) REFERENCES ticktick.users(user_id)
);

CREATE TABLE ticktick.task_tags (
                                    task_id INTEGER NOT NULL,
                                    tag_id INTEGER NOT NULL,
                                    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                    PRIMARY KEY (task_id, tag_id),
                                    FOREIGN KEY (task_id) REFERENCES ticktick.tickticktask(task_id),
                                    FOREIGN KEY (tag_id) REFERENCES ticktick.tags(tag_id)
);

-- Create indexes for better query performance
CREATE INDEX idx_tasks_list_id ON ticktick.tickticktask(list_id);
CREATE INDEX idx_tasks_due_date ON ticktick.tickticktask(due_date);
CREATE INDEX idx_lists_user_id ON ticktick.lists(user_id);
CREATE INDEX idx_tags_user_id ON ticktick.tags(user_id);