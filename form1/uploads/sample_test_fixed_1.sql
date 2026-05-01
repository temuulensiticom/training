-- ============================================================
-- sample_test_fixed.sql - Works with MySqlConnector
-- This version removes DELIMITER and uses standard MySQL syntax
-- ============================================================

CREATE DATABASE IF NOT EXISTS test_schema;
USE test_schema;

DROP TABLE IF EXISTS orders;
DROP TABLE IF EXISTS customers;

-- ── DDL: Create Tables ─────────────────────────────────────
CREATE TABLE customers (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    name        VARCHAR(100) NOT NULL,
    email       VARCHAR(150) UNIQUE,
    phone       VARCHAR(30),
    created_at  DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_email (email)
);

CREATE TABLE orders (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    customer_id INT NOT NULL,
    amount      DECIMAL(10,2),
    status      ENUM('pending','paid','cancelled') DEFAULT 'pending',
    placed_at   DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (customer_id) REFERENCES customers(id) ON DELETE CASCADE
);

-- ── DML: INSERT ───────────────────────────────────────────
INSERT INTO customers (name, email) VALUES ('Alice Smith',  'alice@example.com');
INSERT INTO customers (name, email) VALUES ('Bob Jones',    'bob@example.com');
INSERT INTO customers (name, email) VALUES ('Carol White',  'carol@example.com');

INSERT INTO orders (customer_id, amount, status) VALUES (1, 99.99,  'paid');
INSERT INTO orders (customer_id, amount, status) VALUES (1, 149.00, 'pending');
INSERT INTO orders (customer_id, amount, status) VALUES (2, 55.50,  'cancelled');

-- ── DML: UPDATE / DELETE ──────────────────────────────────
UPDATE orders SET status = 'paid' WHERE status = 'pending';
DELETE FROM orders WHERE status = 'cancelled';

-- ── DDL: ALTER ────────────────────────────────────────────
ALTER TABLE customers ADD COLUMN phone VARCHAR(20);
ALTER TABLE customers MODIFY COLUMN phone VARCHAR(30);

-- ── DDL: VIEW ─────────────────────────────────────────────
CREATE OR REPLACE VIEW v_paid_orders AS
    SELECT c.name, c.email, o.id, o.amount, o.placed_at
    FROM orders o
    JOIN customers c ON c.id = o.customer_id
    WHERE o.status = 'paid';

-- ── CREATE INDEX ──────────────────────────────────────────
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_customer ON orders(customer_id);

-- ── TRANSACTION ───────────────────────────────────────────
START TRANSACTION;
INSERT INTO customers (name, email) VALUES ('Dave TestA', 'dave@example.com');
COMMIT;

-- ── TRANSACTION WITH ROLLBACK ─────────────────────────────
START TRANSACTION;
INSERT INTO customers (name, email) VALUES ('Eve TestB', 'eve@example.com');
ROLLBACK;

-- ── Simple SELECT queries ──────────────────────────────────
SELECT * FROM customers;
SELECT COUNT(*) as total_customers FROM customers;
SELECT id, name, email FROM customers WHERE id > 0;
SELECT status, COUNT(*) as order_count FROM orders GROUP BY status;

-- ── GRANT / permissions example (may need root) ───────────
-- GRANT SELECT ON test_schema.* TO 'appuser'@'%';

-- ── VIEW SELECT ───────────────────────────────────────────
SELECT * FROM v_paid_orders;
