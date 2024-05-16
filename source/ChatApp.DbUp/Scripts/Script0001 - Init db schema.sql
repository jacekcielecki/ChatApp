CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE "users" (
  "id" uuid UNIQUE PRIMARY KEY NOT NULL DEFAULT uuid_generate_v4(),
  "email" varchar UNIQUE NOT NULL,
  "created_at" timestamp NOT NULL
);

CREATE TABLE "messages" (
  "id" uuid UNIQUE PRIMARY KEY NOT NULL DEFAULT uuid_generate_v4(),
  "created_at" timestamp NOT NULL,
  "created_by_id" uuid NOT NULL,
  "content" varchar NOT NULL
);

CREATE TABLE "group_chats" (
  "id" uuid UNIQUE PRIMARY KEY NOT NULL DEFAULT uuid_generate_v4(),
  "name" varchar NOT NULL,
  "created_at" timestamp NOT NULL,
  "created_by_id" uuid NOT NULL
);

CREATE TABLE "group_chats_users" (
  "group_chat_id" uuid NOT NULL,
  "user_id" uuid NOT NULL,
  "created_at" timestamp NOT NULL
);

CREATE TABLE "group_chats_messages" (
  "group_chat_id" uuid NOT NULL,
  "message_id" uuid NOT NULL
);

CREATE TABLE "private_chats" (
  "id" uuid UNIQUE PRIMARY KEY NOT NULL DEFAULT uuid_generate_v4(),
  "name" varchar NOT NULL,
  "created_at" timestamp NOT NULL,
  "first_user_id" uuid NOT NULL,
  "second_user_id" uuid NOT NULL
);

CREATE TABLE "private_chats_messages" (
  "private_chat_id" uuid NOT NULL,
  "message_id" uuid NOT NULL
);

ALTER TABLE "messages" ADD FOREIGN KEY ("created_by_id") REFERENCES "users" ("id");

ALTER TABLE "group_chats" ADD FOREIGN KEY ("created_by_id") REFERENCES "users" ("id");

ALTER TABLE "group_chats_users" ADD FOREIGN KEY ("user_id") REFERENCES "users" ("id");

ALTER TABLE "group_chats_users" ADD FOREIGN KEY ("group_chat_id") REFERENCES "group_chats" ("id");

ALTER TABLE "group_chats_messages" ADD FOREIGN KEY ("message_id") REFERENCES "messages" ("id");

ALTER TABLE "group_chats_messages" ADD FOREIGN KEY ("group_chat_id") REFERENCES "group_chats" ("id");

ALTER TABLE "private_chats" ADD FOREIGN KEY ("first_user_id") REFERENCES "users" ("id");

ALTER TABLE "private_chats" ADD FOREIGN KEY ("second_user_id") REFERENCES "users" ("id");

ALTER TABLE "private_chats_messages" ADD FOREIGN KEY ("private_chat_id") REFERENCES "private_chats" ("id");

ALTER TABLE "private_chats_messages" ADD FOREIGN KEY ("message_id") REFERENCES "messages" ("id");
