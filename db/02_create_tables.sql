--
-- PostgreSQL database dump
--

\restrict a7f16p1NKFQetZcYb1d8VjvMuX7LfzT5jAbNMsg6xSa8lBZxYDFoqesTgNBEEl0

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

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: customer; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.customer (
    id integer NOT NULL,
    customer_uuid uuid NOT NULL,
    name character varying(20) NOT NULL,
    address1 character varying(100) NOT NULL,
    address2 character varying(100),
    phone_number character varying(20) NOT NULL,
    mail_address character varying(200) NOT NULL,
    username character varying(30) NOT NULL,
    password character varying(255) NOT NULL,
    created_at timestamp without time zone NOT NULL,
    kana character varying(20)
);


--
-- Name: TABLE customer; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.customer IS '顧客';


--
-- Name: COLUMN customer.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.customer.id IS '顧客ID';


--
-- Name: COLUMN customer.customer_uuid; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.customer.customer_uuid IS '顧客識別ID';


--
-- Name: COLUMN customer.name; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.customer.name IS '顧客名';


--
-- Name: COLUMN customer.address1; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.customer.address1 IS '住所1';


--
-- Name: COLUMN customer.address2; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.customer.address2 IS '住所2';


--
-- Name: COLUMN customer.phone_number; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.customer.phone_number IS '電話番号';


--
-- Name: COLUMN customer.mail_address; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.customer.mail_address IS 'メールアドレス';


--
-- Name: COLUMN customer.username; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.customer.username IS 'アカウント名';


--
-- Name: COLUMN customer.password; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.customer.password IS 'パスワード（ハッシュ値）';


--
-- Name: COLUMN customer.created_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.customer.created_at IS '登録日';


--
-- Name: customer_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.customer_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: customer_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.customer_id_seq OWNED BY public.customer.id;


--
-- Name: department; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.department (
    id integer NOT NULL,
    department_uuid uuid NOT NULL,
    name character varying(100) NOT NULL
);


--
-- Name: department_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.department_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: department_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.department_id_seq OWNED BY public.department.id;


--
-- Name: employee; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.employee (
    id integer NOT NULL,
    employee_uuid uuid NOT NULL,
    name character varying(100) NOT NULL,
    kana character varying(100) NOT NULL,
    department_id integer NOT NULL
);


--
-- Name: employee_account; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.employee_account (
    id integer NOT NULL,
    account_uuid uuid NOT NULL,
    name character varying(20) NOT NULL,
    password character varying(255) NOT NULL,
    employee_id integer NOT NULL
);


--
-- Name: employee_account_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.employee_account_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: employee_account_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.employee_account_id_seq OWNED BY public.employee_account.id;


--
-- Name: employee_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.employee_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: employee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.employee_id_seq OWNED BY public.employee.id;


--
-- Name: order_status; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.order_status (
    id integer NOT NULL,
    name character varying(100) NOT NULL
);


--
-- Name: TABLE order_status; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.order_status IS '注文ステータス';


--
-- Name: COLUMN order_status.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.order_status.id IS '注文ステータスID';


--
-- Name: COLUMN order_status.name; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.order_status.name IS '注文ステータス名';


--
-- Name: order_status_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.order_status_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: order_status_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.order_status_id_seq OWNED BY public.order_status.id;


--
-- Name: orders; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.orders (
    id integer NOT NULL,
    order_uuid uuid NOT NULL,
    order_date timestamp without time zone NOT NULL,
    amount_total integer NOT NULL,
    customer_id integer NOT NULL,
    order_status_id integer NOT NULL,
    payment_method_id integer NOT NULL
);


--
-- Name: TABLE orders; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.orders IS '注文';


--
-- Name: COLUMN orders.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.orders.id IS '注文ID';


--
-- Name: COLUMN orders.order_uuid; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.orders.order_uuid IS '注文識別ID';


--
-- Name: COLUMN orders.order_date; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.orders.order_date IS '注文日';


--
-- Name: COLUMN orders.amount_total; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.orders.amount_total IS '合計金額';


--
-- Name: COLUMN orders.customer_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.orders.customer_id IS '顧客ID';


--
-- Name: COLUMN orders.order_status_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.orders.order_status_id IS '注文ステータスID';


--
-- Name: COLUMN orders.payment_method_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.orders.payment_method_id IS '支払い方法ID';


--
-- Name: orders_detail; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.orders_detail (
    id integer NOT NULL,
    order_id integer NOT NULL,
    product_id integer NOT NULL,
    count integer NOT NULL
);


--
-- Name: TABLE orders_detail; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.orders_detail IS '注文明細';


--
-- Name: COLUMN orders_detail.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.orders_detail.id IS '注文明細ID';


--
-- Name: COLUMN orders_detail.order_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.orders_detail.order_id IS '注文ID';


--
-- Name: COLUMN orders_detail.product_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.orders_detail.product_id IS '商品ID';


--
-- Name: COLUMN orders_detail.count; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.orders_detail.count IS '注文数';


--
-- Name: orders_detail_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.orders_detail_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: orders_detail_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.orders_detail_id_seq OWNED BY public.orders_detail.id;


--
-- Name: orders_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.orders_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: orders_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.orders_id_seq OWNED BY public.orders.id;


--
-- Name: payment_method; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.payment_method (
    id integer NOT NULL,
    name character varying(100) NOT NULL
);


--
-- Name: TABLE payment_method; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.payment_method IS '支払い方法';


--
-- Name: COLUMN payment_method.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.payment_method.id IS '支払い方法ID';


--
-- Name: COLUMN payment_method.name; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.payment_method.name IS '支払い方法名';


--
-- Name: payment_method_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.payment_method_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: payment_method_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.payment_method_id_seq OWNED BY public.payment_method.id;


--
-- Name: product; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.product (
    id integer NOT NULL,
    product_uuid uuid NOT NULL,
    name character varying(100) NOT NULL,
    price integer NOT NULL,
    image_url character varying(200),
    product_category_id integer NOT NULL,
    delete_flg integer DEFAULT 0 NOT NULL
);


--
-- Name: TABLE product; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.product IS '商品';


--
-- Name: COLUMN product.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.product.id IS '商品ID';


--
-- Name: COLUMN product.product_uuid; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.product.product_uuid IS '商品識別ID';


--
-- Name: COLUMN product.name; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.product.name IS '商品名';


--
-- Name: COLUMN product.price; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.product.price IS '価格';


--
-- Name: COLUMN product.image_url; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.product.image_url IS '画像URL';


--
-- Name: COLUMN product.product_category_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.product.product_category_id IS '商品カテゴリID';


--
-- Name: COLUMN product.delete_flg; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.product.delete_flg IS '削除フラグ(0:有効 1:削除)';


--
-- Name: product_category; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.product_category (
    id integer NOT NULL,
    category_uuid uuid NOT NULL,
    name character varying(30) NOT NULL
);


--
-- Name: product_category_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.product_category_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: product_category_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.product_category_id_seq OWNED BY public.product_category.id;


--
-- Name: product_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.product_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: product_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.product_id_seq OWNED BY public.product.id;


--
-- Name: product_stock; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.product_stock (
    id integer NOT NULL,
    stock_uuid uuid NOT NULL,
    quantity integer NOT NULL,
    product_id integer NOT NULL
);


--
-- Name: TABLE product_stock; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.product_stock IS '商品在庫';


--
-- Name: COLUMN product_stock.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.product_stock.id IS '商品在庫ID';


--
-- Name: COLUMN product_stock.stock_uuid; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.product_stock.stock_uuid IS '商品在庫識別ID';


--
-- Name: COLUMN product_stock.quantity; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.product_stock.quantity IS '商品在庫数';


--
-- Name: COLUMN product_stock.product_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.product_stock.product_id IS '商品ID';


--
-- Name: product_stock_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.product_stock_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: product_stock_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.product_stock_id_seq OWNED BY public.product_stock.id;


--
-- Name: customer id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.customer ALTER COLUMN id SET DEFAULT nextval('public.customer_id_seq'::regclass);


--
-- Name: department id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.department ALTER COLUMN id SET DEFAULT nextval('public.department_id_seq'::regclass);


--
-- Name: employee id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.employee ALTER COLUMN id SET DEFAULT nextval('public.employee_id_seq'::regclass);


--
-- Name: employee_account id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.employee_account ALTER COLUMN id SET DEFAULT nextval('public.employee_account_id_seq'::regclass);


--
-- Name: order_status id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.order_status ALTER COLUMN id SET DEFAULT nextval('public.order_status_id_seq'::regclass);


--
-- Name: orders id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders ALTER COLUMN id SET DEFAULT nextval('public.orders_id_seq'::regclass);


--
-- Name: orders_detail id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders_detail ALTER COLUMN id SET DEFAULT nextval('public.orders_detail_id_seq'::regclass);


--
-- Name: payment_method id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.payment_method ALTER COLUMN id SET DEFAULT nextval('public.payment_method_id_seq'::regclass);


--
-- Name: product id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.product ALTER COLUMN id SET DEFAULT nextval('public.product_id_seq'::regclass);


--
-- Name: product_category id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.product_category ALTER COLUMN id SET DEFAULT nextval('public.product_category_id_seq'::regclass);


--
-- Name: product_stock id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.product_stock ALTER COLUMN id SET DEFAULT nextval('public.product_stock_id_seq'::regclass);


--
-- Name: customer pk_customer; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT pk_customer PRIMARY KEY (id);


--
-- Name: department pk_department; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.department
    ADD CONSTRAINT pk_department PRIMARY KEY (id);


--
-- Name: employee pk_employee; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT pk_employee PRIMARY KEY (id);


--
-- Name: employee_account pk_employee_account; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.employee_account
    ADD CONSTRAINT pk_employee_account PRIMARY KEY (id);


--
-- Name: order_status pk_order_status; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.order_status
    ADD CONSTRAINT pk_order_status PRIMARY KEY (id);


--
-- Name: orders pk_orders; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT pk_orders PRIMARY KEY (id);


--
-- Name: orders_detail pk_orders_detail; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders_detail
    ADD CONSTRAINT pk_orders_detail PRIMARY KEY (id);


--
-- Name: payment_method pk_payment_method; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.payment_method
    ADD CONSTRAINT pk_payment_method PRIMARY KEY (id);


--
-- Name: product pk_product; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT pk_product PRIMARY KEY (id);


--
-- Name: product_category pk_product_category; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.product_category
    ADD CONSTRAINT pk_product_category PRIMARY KEY (id);


--
-- Name: product_stock pk_product_stock; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.product_stock
    ADD CONSTRAINT pk_product_stock PRIMARY KEY (id);


--
-- Name: customer uk_customer_mail; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT uk_customer_mail UNIQUE (mail_address);


--
-- Name: customer uk_customer_username; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT uk_customer_username UNIQUE (username);


--
-- Name: customer uk_customer_uuid; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT uk_customer_uuid UNIQUE (customer_uuid);


--
-- Name: department uk_department_uuid; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.department
    ADD CONSTRAINT uk_department_uuid UNIQUE (department_uuid);


--
-- Name: employee_account uk_employee_account_uuid; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.employee_account
    ADD CONSTRAINT uk_employee_account_uuid UNIQUE (account_uuid);


--
-- Name: employee uk_employee_uuid; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT uk_employee_uuid UNIQUE (employee_uuid);


--
-- Name: orders uk_orders_uuid; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT uk_orders_uuid UNIQUE (order_uuid);


--
-- Name: product_category uk_product_category_uuid; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.product_category
    ADD CONSTRAINT uk_product_category_uuid UNIQUE (category_uuid);


--
-- Name: product_stock uk_product_stock_uuid; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.product_stock
    ADD CONSTRAINT uk_product_stock_uuid UNIQUE (stock_uuid);


--
-- Name: product uk_product_uuid; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT uk_product_uuid UNIQUE (product_uuid);


--
-- Name: employee_account fk_employee_account_employee; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.employee_account
    ADD CONSTRAINT fk_employee_account_employee FOREIGN KEY (employee_id) REFERENCES public.employee(id);


--
-- Name: employee fk_employee_department; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT fk_employee_department FOREIGN KEY (department_id) REFERENCES public.department(id);


--
-- Name: orders fk_orders_customer; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_customer FOREIGN KEY (customer_id) REFERENCES public.customer(id);


--
-- Name: orders_detail fk_orders_detail_order; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders_detail
    ADD CONSTRAINT fk_orders_detail_order FOREIGN KEY (order_id) REFERENCES public.orders(id);


--
-- Name: orders_detail fk_orders_detail_product; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders_detail
    ADD CONSTRAINT fk_orders_detail_product FOREIGN KEY (product_id) REFERENCES public.product(id);


--
-- Name: orders fk_orders_order_status; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_order_status FOREIGN KEY (order_status_id) REFERENCES public.order_status(id);


--
-- Name: orders fk_orders_payment_method; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_payment_method FOREIGN KEY (payment_method_id) REFERENCES public.payment_method(id);


--
-- Name: product fk_product_category; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT fk_product_category FOREIGN KEY (product_category_id) REFERENCES public.product_category(id);


--
-- Name: product_stock fk_product_stock_product; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.product_stock
    ADD CONSTRAINT fk_product_stock_product FOREIGN KEY (product_id) REFERENCES public.product(id);


--
-- PostgreSQL database dump complete
--

\unrestrict a7f16p1NKFQetZcYb1d8VjvMuX7LfzT5jAbNMsg6xSa8lBZxYDFoqesTgNBEEl0