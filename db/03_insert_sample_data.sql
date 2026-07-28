--
-- PostgreSQL database dump
--

\restrict OdMTLfRpbMS23lvpH7PcX1xcfwwNBbsfNGrV4RpBdMQxB4hH81IG9QgL4Chmgj8

-- Dumped from database version 17.9 (Ubuntu 17.9-1.pgdg24.04+1)
-- Dumped by pg_dump version 17.9 (Ubuntu 17.9-1.pgdg24.04+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Data for Name: customer; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.customer (id, customer_uuid, name, address1, address2, phone_number, mail_address, username, password, created_at, kana) VALUES (1, '11111111-1111-1111-1111-111111111111', '山田 太郎', '東京都新宿区', '西新宿1-1-1', '09011112222', 'taro@example.com', 'taro', 'password', '2026-07-08 16:54:02.152367', 'ヤマダ タロウ');
INSERT INTO public.customer (id, customer_uuid, name, address1, address2, phone_number, mail_address, username, password, created_at, kana) VALUES (2, '22222222-2222-2222-2222-222222222222', '佐藤 花子', '埼玉県さいたま市', '大宮1-2-3', '09022223333', 'hanako@example.com', 'hanako', 'password', '2026-07-08 16:54:02.152367', 'サトウ ハナコ');
INSERT INTO public.customer (id, customer_uuid, name, address1, address2, phone_number, mail_address, username, password, created_at, kana) VALUES (3, '33333333-3333-3333-3333-333333333333', '鈴木 一郎', '千葉県船橋市', '本町3-4-5', '09033334444', 'ichiro@example.com', 'ichiro', 'password', '2026-07-08 16:54:02.152367', 'スズキ イチロウ');


--
-- Data for Name: department; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.department (id, department_uuid, name) VALUES (1, 'e480fa43-f51c-4738-93dc-fb4fe0ecea42', '営業部');
INSERT INTO public.department (id, department_uuid, name) VALUES (2, '3d2639a4-5c77-4737-81de-cf80cfce21de', '総務部');
INSERT INTO public.department (id, department_uuid, name) VALUES (3, '6e06cad7-09e6-4eae-adbe-20102ea58efc', '開発部');


--
-- Data for Name: employee; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.employee (id, employee_uuid, name, kana, department_id) VALUES (1, '11111111-1111-1111-1111-111111111111', '山田 太郎', 'ヤマダ タロウ', 1);
INSERT INTO public.employee (id, employee_uuid, name, kana, department_id) VALUES (2, '22222222-2222-2222-2222-222222222222', '佐藤 花子', 'サトウ ハナコ', 2);
INSERT INTO public.employee (id, employee_uuid, name, kana, department_id) VALUES (3, '33333333-3333-3333-3333-333333333333', '鈴木 一郎', 'スズキ イチロウ', 3);


--
-- Data for Name: employee_account; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.employee_account (id, account_uuid, name, password, employee_id) VALUES (1, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'sato_hanako', 'password', 2);


--
-- Data for Name: order_status; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.order_status (id, name) VALUES (1, '受付');
INSERT INTO public.order_status (id, name) VALUES (2, '支払待ち');
INSERT INTO public.order_status (id, name) VALUES (3, '発送準備中');
INSERT INTO public.order_status (id, name) VALUES (4, '発送済み');
INSERT INTO public.order_status (id, name) VALUES (5, '配達完了');
INSERT INTO public.order_status (id, name) VALUES (6, 'キャンセル');


--
-- Data for Name: payment_method; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.payment_method (id, name) VALUES (1, 'クレジットカード');
INSERT INTO public.payment_method (id, name) VALUES (2, 'PayPay');
INSERT INTO public.payment_method (id, name) VALUES (3, 'コンビニ払い');
INSERT INTO public.payment_method (id, name) VALUES (4, '銀行振込');


--
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.orders (id, order_uuid, order_date, amount_total, customer_id, order_status_id, payment_method_id) VALUES (1, 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '2026-06-28 16:54:25.994352', 3800, 1, 3, 1);
INSERT INTO public.orders (id, order_uuid, order_date, amount_total, customer_id, order_status_id, payment_method_id) VALUES (2, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '2026-07-01 16:54:25.994352', 2500, 1, 3, 2);
INSERT INTO public.orders (id, order_uuid, order_date, amount_total, customer_id, order_status_id, payment_method_id) VALUES (3, 'cccccccc-cccc-cccc-cccc-cccccccccccc', '2026-07-03 16:54:25.994352', 1800, 2, 2, 1);
INSERT INTO public.orders (id, order_uuid, order_date, amount_total, customer_id, order_status_id, payment_method_id) VALUES (5, 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '2026-07-08 16:54:25.994352', 900, 2, 4, 2);
INSERT INTO public.orders (id, order_uuid, order_date, amount_total, customer_id, order_status_id, payment_method_id) VALUES (4, 'dddddddd-dddd-dddd-dddd-dddddddddddd', '2026-07-06 16:54:25.994352', 5200, 3, 2, 3);


--
-- Data for Name: product_category; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.product_category (id, category_uuid, name) VALUES (1, 'e50d978b-b73d-4afb-8e85-ace9cf1e12a7', '文房具');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (2, 'ae4ed829-7017-4972-8187-59384e0b5627', '雑貨');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (3, '707c67f1-8f9a-457f-af39-f99c66085c45', 'パソコン周辺機器');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (8, 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'テストカテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (15, 'e42819e1-ef16-4c7f-8fef-7c1d817f471d', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (16, 'e2756448-babe-4751-a34f-04aac4c6528d', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (17, '2496a18d-5986-42ac-bd31-3a1dd7ead072', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (18, '9b8b5f73-c4c7-42e4-858f-8825ebc20b6f', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (19, '707dde54-a1b2-40d6-ac8c-8509b9ca8416', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (20, 'e58401a8-6cb7-4867-afb6-af0f058e8155', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (21, 'ef593577-0ee9-42f0-b9ff-04a5efc487c0', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (22, '4fcb1e4d-8606-48a2-9b97-f73e0aca5262', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (23, '484c4d15-b8e1-410c-89de-5020d7563c54', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (24, 'a2497996-4c87-4439-ac40-aaef3ccfadf7', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (25, '0ad88831-4272-4902-ad3c-3ac28606dfc8', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (26, '53f68d16-c254-4bcf-ade9-49573ad9e140', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (27, '53fde020-9ce9-4f33-8499-c61a6b9f9796', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (28, 'cff9fea6-1d60-43d7-9077-8b1115c9b961', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (29, '86c6ce50-500c-4888-b5c5-50721ad13d7c', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (30, '4d6c2b4b-3b8a-4f46-a33a-7efdd9d8ac68', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (31, 'c8915bd0-48f2-4f25-85e5-bd3eb3aef306', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (32, '9d75f107-c8f5-4949-ac0d-abe65f473637', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (33, 'babeba4c-a893-4328-9cfb-4c8ae99c9315', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (34, '903370e2-0389-422b-b94d-d684d2ddfd23', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (35, '734b738f-d335-4906-b4e4-e1bc13131937', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (36, 'c28b0231-e064-4f86-9fa3-468cab28f698', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (37, '0ac43fff-9098-4eb6-b52b-d443b609c2f7', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (38, 'ca43b8bd-181f-4e39-8a60-209d5c33144a', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (39, 'b1eb2fe3-8bcf-4ac1-8688-411b95847c0b', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (40, '885f183c-fc40-4ce6-884f-4604224d4e89', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (41, '4c4c990b-bc61-4e1a-9c32-f878dd4c213a', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (42, '9b0b6929-d1aa-4b79-a0a5-1aa3235bd53e', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (43, '7af9a261-fcea-4d17-9eba-66abcf864db9', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (44, '63eee60d-1b62-41fa-b9d8-ebcae6a3570f', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (45, 'd5b14ed2-2cd4-4d2c-8fa5-5630af90d720', '化粧品');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (46, '3a2b3eb1-a791-4c91-b15f-86a45b4f6db1', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (47, '41810869-8f36-4751-9c45-bea59205ed80', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (48, '8195f145-7d21-4622-8648-5e574aae1946', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (49, '7f78ec5c-dcd1-4a53-a451-a3620f449873', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (50, 'a976314a-2fa1-47c5-940c-bb9604d0e8de', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (51, 'a6ac7fe5-63ab-4613-8860-de0e97950260', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (52, '5b822fc6-01be-4f7b-b553-943c05ace9fb', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (53, 'f65ac737-8112-49e6-8716-fc3c5a18e0ed', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (54, '5f9d83b1-1ab8-408a-a81d-cd43b11f685b', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (55, '3c52fc1a-966a-4e44-b64c-f135c732c873', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (56, '5c3b02c8-1276-4a84-bce1-53afa22ed0d0', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (57, '6d992e6c-65fa-4caf-a635-81c8a4c2be9a', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (58, '0bb72667-f2d1-45e6-957f-0f056276edd0', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (59, '4055fedf-3bc4-4a08-9cc0-8f797010ac74', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (60, '7b8c9973-d65d-4151-b726-47ea77c24b25', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (61, '81840c83-3932-4ab3-aaf4-7592386b7b57', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (62, 'd2afbf90-8028-4a21-b4a8-49463a6a310e', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (63, '202a4af2-ee7c-42a4-81f3-1095cf891034', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (64, '67274af6-3e38-4d57-b645-6f50f1183524', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (65, '96a4fd1b-66e4-43c3-bd05-1ed8f8946fef', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (66, 'a14d4563-de11-41ef-b252-33e4b983397a', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (67, '99ecbb99-1fdb-4d3a-9b1a-3e58f011f0b0', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (68, '69c4bbdc-8c50-46a9-b89d-90b338e7429d', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (69, '7cd65049-4212-4efd-b749-41df57c0b567', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (70, '84e394b0-4f6c-416d-97fa-f0080bf03c5e', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (71, '8f80b190-cc34-4135-a13c-51dafef2aedb', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (72, '3c4507c9-cec5-44ec-a1d0-d1b40e91b9ab', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (73, '10019b8b-5c25-4295-9710-54d339fc7f7e', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (74, '15b7ec6d-0f49-48bb-90ac-769271152f98', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (75, '7692a31e-1aa5-4ed0-a3e8-ef014b85567e', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (76, 'aa0f3b45-e821-4dc2-bf85-81cdb78d640e', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (77, 'bd5b45c5-24dd-434b-a00e-ac776a00d76b', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (78, 'ce970d9c-d89a-4502-b64f-0c0d32078ed9', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (79, 'd496f4c7-1c3f-49df-837c-3c6e523324d5', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (80, 'c2830260-b636-47f1-ad91-b8fb074e1764', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (81, '18286e54-09b0-4fff-abab-e3e0702b6a8e', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (82, 'c4232010-9f99-4940-b164-eddd812ec0a0', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (83, '366c2cb3-bbfa-4a9d-a41c-f8fe5bf6822a', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (84, 'edaedc34-c748-4875-a99e-d7af951bc11d', '新規カテゴリ');
INSERT INTO public.product_category (id, category_uuid, name) VALUES (85, '29f9734d-62bd-4d38-a1e5-e4b6fa76f844', '新規カテゴリ');


--
-- Data for Name: product; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.product (id, product_uuid, name, price, image_url, product_category_id, delete_flg) VALUES (137, 'd99baaba-cfb4-4096-ba8c-3ac6f583b9a1', 'テスト商品', 100, NULL, 1, 1);
INSERT INTO public.product (id, product_uuid, name, price, image_url, product_category_id, delete_flg) VALUES (140, '50d72059-08dd-4517-a073-472bf5cd569a', 'テスト商品', 100, NULL, 1, 0);
INSERT INTO public.product (id, product_uuid, name, price, image_url, product_category_id, delete_flg) VALUES (144, '36f3d1d1-f9ba-48eb-971a-a3cd1c16944d', 'テスト商品', 100, NULL, 1, 1);
INSERT INTO public.product (id, product_uuid, name, price, image_url, product_category_id, delete_flg) VALUES (54, '10000000-0000-0000-0000-000000000005', 'ワンタッチ開閉傘', 1980, NULL, 2, 0);
INSERT INTO public.product (id, product_uuid, name, price, image_url, product_category_id, delete_flg) VALUES (55, '10000000-0000-0000-0000-000000000006', 'レザーネックレス', 2980, NULL, 2, 0);
INSERT INTO public.product (id, product_uuid, name, price, image_url, product_category_id, delete_flg) VALUES (50, '10000000-0000-0000-0000-000000000001', '水性ボールペン(黒)', 120, NULL, 1, 0);
INSERT INTO public.product (id, product_uuid, name, price, image_url, product_category_id, delete_flg) VALUES (51, '10000000-0000-0000-0000-000000000002', '水性ボールペン(赤)', 120, NULL, 1, 0);
INSERT INTO public.product (id, product_uuid, name, price, image_url, product_category_id, delete_flg) VALUES (52, '10000000-0000-0000-0000-000000000003', 'ワイヤレスマウス', 2480, NULL, 3, 0);
INSERT INTO public.product (id, product_uuid, name, price, image_url, product_category_id, delete_flg) VALUES (53, '10000000-0000-0000-0000-000000000004', 'ワイヤレストラックボール', 3980, NULL, 3, 0);


--
-- Data for Name: orders_detail; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.orders_detail (id, order_id, product_id, count) VALUES (1, 1, 50, 2);
INSERT INTO public.orders_detail (id, order_id, product_id, count) VALUES (2, 1, 51, 1);
INSERT INTO public.orders_detail (id, order_id, product_id, count) VALUES (3, 2, 52, 1);
INSERT INTO public.orders_detail (id, order_id, product_id, count) VALUES (4, 2, 53, 1);
INSERT INTO public.orders_detail (id, order_id, product_id, count) VALUES (5, 3, 54, 1);
INSERT INTO public.orders_detail (id, order_id, product_id, count) VALUES (6, 3, 55, 1);
INSERT INTO public.orders_detail (id, order_id, product_id, count) VALUES (7, 4, 50, 5);
INSERT INTO public.orders_detail (id, order_id, product_id, count) VALUES (8, 4, 51, 5);
INSERT INTO public.orders_detail (id, order_id, product_id, count) VALUES (9, 4, 52, 1);
INSERT INTO public.orders_detail (id, order_id, product_id, count) VALUES (10, 5, 53, 1);


--
-- Data for Name: product_stock; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.product_stock (id, stock_uuid, quantity, product_id) VALUES (1, '20000000-0000-0000-0000-000000000001', 80, 50);
INSERT INTO public.product_stock (id, stock_uuid, quantity, product_id) VALUES (2, '20000000-0000-0000-0000-000000000002', 65, 51);
INSERT INTO public.product_stock (id, stock_uuid, quantity, product_id) VALUES (3, '20000000-0000-0000-0000-000000000003', 30, 52);
INSERT INTO public.product_stock (id, stock_uuid, quantity, product_id) VALUES (4, '20000000-0000-0000-0000-000000000004', 18, 53);
INSERT INTO public.product_stock (id, stock_uuid, quantity, product_id) VALUES (5, '20000000-0000-0000-0000-000000000005', 25, 54);
INSERT INTO public.product_stock (id, stock_uuid, quantity, product_id) VALUES (6, '20000000-0000-0000-0000-000000000006', 12, 55);
INSERT INTO public.product_stock (id, stock_uuid, quantity, product_id) VALUES (88, 'ec4acd51-4420-413e-b6b3-fa04cdf7b50d', 10, 137);
INSERT INTO public.product_stock (id, stock_uuid, quantity, product_id) VALUES (92, '2ff09356-85cf-4264-91b3-ad52ac0a63ec', 10, 140);
INSERT INTO public.product_stock (id, stock_uuid, quantity, product_id) VALUES (95, 'd8453a48-864f-42b5-930f-1b604a46df54', 10, 144);


--
-- Name: customer_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.customer_id_seq', 10, true);


--
-- Name: department_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.department_id_seq', 3, true);


--
-- Name: employee_account_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.employee_account_id_seq', 71, true);


--
-- Name: employee_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.employee_id_seq', 3, true);


--
-- Name: order_status_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.order_status_id_seq', 6, true);


--
-- Name: orders_detail_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.orders_detail_id_seq', 1, false);


--
-- Name: orders_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.orders_id_seq', 1, false);


--
-- Name: payment_method_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.payment_method_id_seq', 4, true);


--
-- Name: product_category_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.product_category_id_seq', 110, true);


--
-- Name: product_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.product_id_seq', 358, true);


--
-- Name: product_stock_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.product_stock_id_seq', 309, true);


--
-- PostgreSQL database dump complete
--

\unrestrict OdMTLfRpbMS23lvpH7PcX1xcfwwNBbsfNGrV4RpBdMQxB4hH81IG9QgL4Chmgj8