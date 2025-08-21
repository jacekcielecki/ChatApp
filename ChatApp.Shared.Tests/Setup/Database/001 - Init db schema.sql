CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE "users" (
  "id" uuid UNIQUE PRIMARY KEY NOT NULL DEFAULT (uuid_generate_v4()),
  "email" varchar UNIQUE NOT NULL,
  "created_at" timestamp NOT NULL
);

CREATE TABLE "messages" (
  "id" uuid UNIQUE PRIMARY KEY NOT NULL DEFAULT (uuid_generate_v4()),
  "created_at" timestamp NOT NULL,
  "chat_id" uuid NOT NULL,
  "created_by_id" uuid NOT NULL,
  "content" varchar NOT NULL,
  FOREIGN KEY ("created_by_id") REFERENCES "users" ("id")
);

CREATE TABLE "group_chats" (
  "id" uuid UNIQUE PRIMARY KEY NOT NULL DEFAULT (uuid_generate_v4()),
  "name" varchar NOT NULL,
  "created_at" timestamp NOT NULL,
  "created_by_id" uuid NOT NULL,
  FOREIGN KEY ("created_by_id") REFERENCES "users" ("id")
);

CREATE TABLE "group_chats_users" (
  "group_chat_id" uuid NOT NULL,
  "user_id" uuid NOT NULL,
  "created_at" timestamp NOT NULL,
  FOREIGN KEY ("user_id") REFERENCES "users" ("id"),
  FOREIGN KEY ("group_chat_id") REFERENCES "group_chats" ("id")
);

CREATE TABLE "private_chats" (
  "id" uuid UNIQUE PRIMARY KEY NOT NULL DEFAULT (uuid_generate_v4()),
  "created_at" timestamp NOT NULL,
  "first_user_id" uuid NOT NULL,
  "second_user_id" uuid NOT NULL,
  FOREIGN KEY ("first_user_id") REFERENCES "users" ("id"),
  FOREIGN KEY ("second_user_id") REFERENCES "users" ("id")
);
