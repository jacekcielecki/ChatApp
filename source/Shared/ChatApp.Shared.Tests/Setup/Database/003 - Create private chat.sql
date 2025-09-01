INSERT INTO "private_chats" ("id", "created_at", "first_user_id", "second_user_id")
VALUES ('4398407C-7AAB-40AA-A88A-618B7E4F5701', NOW(), '98778b84-6108-45c0-b4b9-a7ac71059ce5', '98778b84-6108-45c0-b4b9-a7ac71059ce5');

INSERT INTO "messages" ("id", "created_at", "chat_id", "created_by_id", "content") VALUES
('3d430053-b960-4c51-b685-348feac08016', NOW(), '4398407C-7AAB-40AA-A88A-618B7E4F5701', '67edf34c-2203-4052-9c0b-9eb400d9d6e2', 'Hey David, how are you?'),
('cf1038bd-3680-41dd-9cb4-ed4a7644f8b8', NOW(), '4398407C-7AAB-40AA-A88A-618B7E4F5701', '98778b84-6108-45c0-b4b9-a7ac71059ce5', 'Hi Charlie! I am good, thanks. How about you?'),
('b5121b07-6a56-49c2-aef7-0e6c5ac13c2d', NOW(), '4398407C-7AAB-40AA-A88A-618B7E4F5701', '67edf34c-2203-4052-9c0b-9eb400d9d6e2', 'I am doing well! Been keeping busy with work lately.'),
('a4ff62b3-2d18-4c13-94d2-38a4d71a8b91', NOW(), '4398407C-7AAB-40AA-A88A-618B7E4F5701', '98778b84-6108-45c0-b4b9-a7ac71059ce5', 'Same here! Lots of projects going on. Did you do anything fun over the weekend?'),
('0f75df49-9e8e-4b15-90d6-b1f7c6f58741', NOW(), '4398407C-7AAB-40AA-A88A-618B7E4F5701', '67edf34c-2203-4052-9c0b-9eb400d9d6e2', 'Yeah, I went hiking on Saturday. The weather was perfect for it.'),
('d78a5f1e-0c17-4e2a-8f33-9d32c55d4418', NOW(), '4398407C-7AAB-40AA-A88A-618B7E4F5701', '98778b84-6108-45c0-b4b9-a7ac71059ce5', 'That sounds great! I’ve been meaning to get outdoors more. Which trail did you go to?'),
('1a3e441d-062a-4f67-9373-02a58b39a11d', NOW(), '4398407C-7AAB-40AA-A88A-618B7E4F5701', '67edf34c-2203-4052-9c0b-9eb400d9d6e2', 'I went to Pine Ridge Trail. It has amazing views at the top. We should go together sometime!'),
('44d46a38-2d95-4b90-bf9a-5a3b782fa85a', NOW(), '4398407C-7AAB-40AA-A88A-618B7E4F5701', '98778b84-6108-45c0-b4b9-a7ac71059ce5', 'I would love that! Let’s plan for next weekend if the weather holds up.');
