DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'ticktick') THEN
        CREATE SCHEMA ticktick;
    END IF;
END $EF$;


CREATE TABLE ticktick.users (
    user_id text NOT NULL,
    email text NOT NULL,
    salt text NOT NULL,
    password_hash text NOT NULL,
    role text NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT users_pkey PRIMARY KEY (user_id)
);


CREATE TABLE ticktick.tags (
    tag_id text NOT NULL,
    name text NOT NULL,
    user_id text NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT tags_pkey PRIMARY KEY (tag_id),
    CONSTRAINT tags_user_id_fkey FOREIGN KEY (user_id) REFERENCES ticktick.users (user_id)
);


CREATE TABLE ticktick.tasklist (
    list_id text NOT NULL,
    user_id text NOT NULL,
    name text NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT tasklist_pkey PRIMARY KEY (list_id),
    CONSTRAINT tasklist_user_id_fkey FOREIGN KEY (user_id) REFERENCES ticktick.users (user_id)
);


CREATE TABLE ticktick.tickticktask (
    task_id text NOT NULL,
    list_id text NOT NULL,
    title text NOT NULL,
    description text NOT NULL,
    due_date timestamp with time zone,
    priority integer NOT NULL,
    completed boolean NOT NULL,
    created_at timestamp with time zone NOT NULL,
    completed_at timestamp with time zone,
    CONSTRAINT tickticktask_pkey PRIMARY KEY (task_id),
    CONSTRAINT tickticktask_list_id_fkey FOREIGN KEY (list_id) REFERENCES ticktick.tasklist (list_id)
);


CREATE TABLE ticktick.task_tags (
    task_id text NOT NULL,
    tag_id text NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT task_tags_pkey PRIMARY KEY (task_id, tag_id),
    CONSTRAINT task_tags_tag_id_fkey FOREIGN KEY (tag_id) REFERENCES ticktick.tags (tag_id),
    CONSTRAINT task_tags_task_id_fkey FOREIGN KEY (task_id) REFERENCES ticktick.tickticktask (task_id)
);


CREATE INDEX idx_tags_user_id ON ticktick.tags (user_id);


CREATE INDEX "IX_task_tags_tag_id" ON ticktick.task_tags (tag_id);


CREATE INDEX idx_lists_user_id ON ticktick.tasklist (user_id);


CREATE INDEX idx_tasks_due_date ON ticktick.tickticktask (due_date);


CREATE INDEX idx_tasks_list_id ON ticktick.tickticktask (list_id);


