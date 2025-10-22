INSERT INTO "group_chats" ("created_at", "name", "created_by_id", "id")
VALUES (NOW(), 'Study Group', 'a6e58f6e-fef3-458c-a26c-19cfc14d329e', 'C653AF32-7980-4480-ACF2-708A0653ECBA');

INSERT INTO "group_chats_users" ("group_chat_id", "user_id", "created_at") VALUES
('c653af32-7980-4480-acf2-708a0653ecba', 'a6e58f6e-fef3-458c-a26c-19cfc14d329e', now()),
('c653af32-7980-4480-acf2-708a0653ecba', '9151458c-3010-4463-8d40-c3722f061704', now()),
('c653af32-7980-4480-acf2-708a0653ecba', '67edf34c-2203-4052-9c0b-9eb400d9d6e2', now()),
('c653af32-7980-4480-acf2-708a0653ecba', '98778b84-6108-45c0-b4b9-a7ac71059ce5', now()),
('c653af32-7980-4480-acf2-708a0653ecba', 'ff32d4d0-86ca-41be-9157-9a2ce3d5bcd4', now());

INSERT INTO "messages" ("id", "created_at", "chat_id", "created_by_id", "content") VALUES
('E53CCAEA-709F-4BC8-9B36-85EB22E24C8C', NOW(), 'C653AF32-7980-4480-ACF2-708A0653ECBA', '9151458c-3010-4463-8d40-c3722f061704', 'Hey David, how are you?'),
('4FEE255A-CF88-4A87-8C11-D730274B0325', NOW(), 'C653AF32-7980-4480-ACF2-708A0653ECBA', '98778b84-6108-45c0-b4b9-a7ac71059ce5', 'Hi Bob! I am good, thanks. How about you?'),
('673968ED-2663-43FB-BD6A-2C36A297942C', NOW(), 'C653AF32-7980-4480-ACF2-708A0653ECBA', '9151458c-3010-4463-8d40-c3722f061704', 'Doing great, just working on a project.'),
('989AB14F-9210-4A51-8EA6-256B553DA825', NOW(), 'C653AF32-7980-4480-ACF2-708A0653ECBA', '98778b84-6108-45c0-b4b9-a7ac71059ce5', 'Nice! Let me know if you need help.');
