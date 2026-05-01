-- ============================================================
-- sample_test.sql  –  Tests every major MySQL action type
-- Run this twice to verify duplicate/exists handling
-- ============================================================

-- ── DDL: Database & Tables ────────────────────────────────
CREATE DATABASE IF NOT EXISTS test_schema;
USE test_schema;

DROP TABLE IF EXISTS orders;
DROP TABLE IF EXISTS customers;

CREATE TABLE customers (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    name        VARCHAR(100) NOT NULL,
    email       VARCHAR(150) UNIQUE,
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
INSERT INTO customers (name, email) VALUES
  ('Alice Smith',  'alice@example.com'),
  ('Bob Jones',    'bob@example.com'),
  ('Carol White',  'carol@example.com');

INSERT INTO orders (customer_id, amount, status) VALUES
  (1, 99.99,  'paid'),
  (1, 149.00, 'pending'),
  (2, 55.50,  'cancelled');

-- ── DML: UPDATE / DELETE ──────────────────────────────────
UPDATE orders SET status = 'paid' WHERE status = 'pending';
DELETE FROM orders WHERE status = 'cancelled';

-- ── DDL: ALTER ────────────────────────────────────────────
ALTER TABLE customers ADD COLUMN phone VARCHAR(20) AFTER email;
ALTER TABLE customers MODIFY COLUMN phone VARCHAR(30);

-- ── DDL: VIEW ─────────────────────────────────────────────
CREATE OR REPLACE VIEW v_paid_orders AS
    SELECT c.name, o.amount, o.placed_at
    FROM orders o
    JOIN customers c ON c.id = o.customer_id
    WHERE o.status = 'paid';

-- ── STORED PROCEDURE ──────────────────────────────────────
DELIMITER $$

DROP PROCEDURE IF EXISTS sp_get_customer_orders$$

CREATE PROCEDURE sp_get_customer_orders(IN p_customer_id INT)
BEGIN
    SELECT o.id, o.amount, o.status, o.placed_at
    FROM orders o
    WHERE o.customer_id = p_customer_id
    ORDER BY o.placed_at DESC;
END$$

-- ── PROCEDURE with OUT param ──────────────────────────────
DROP PROCEDURE IF EXISTS sp_order_count$$

CREATE PROCEDURE sp_order_count(IN p_customer_id INT, OUT p_count INT)
BEGIN
    SELECT COUNT(*) INTO p_count
    FROM orders
    WHERE customer_id = p_customer_id;
END$$

-- ── FUNCTION ──────────────────────────────────────────────
DROP FUNCTION IF EXISTS fn_customer_total$$

CREATE FUNCTION fn_customer_total(p_customer_id INT)
RETURNS DECIMAL(10,2)
READS SQL DATA
DETERMINISTIC
BEGIN
    DECLARE total DECIMAL(10,2);
    SELECT COALESCE(SUM(amount), 0) INTO total
    FROM orders
    WHERE customer_id = p_customer_id AND status = 'paid';
    RETURN total;
END$$

-- ── TRIGGER ───────────────────────────────────────────────
DROP TRIGGER IF EXISTS trg_orders_before_insert$$

CREATE TRIGGER trg_orders_before_insert
BEFORE INSERT ON orders
FOR EACH ROW
BEGIN
    IF NEW.amount <= 0 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Order amount must be greater than zero';
    END IF;
END$$

-- ── EVENT ─────────────────────────────────────────────────
DROP EVENT IF EXISTS evt_cleanup_old_orders$$

CREATE EVENT evt_cleanup_old_orders
ON SCHEDULE EVERY 1 DAY
STARTS CURRENT_TIMESTAMP
DO
BEGIN
    DELETE FROM orders
    WHERE status = 'cancelled'
      AND placed_at < DATE_SUB(NOW(), INTERVAL 30 DAY);
END$$

DELIMITER ;

-- ── CALL stored procedures ────────────────────────────────
CALL sp_get_customer_orders(1);

-- ── DQL: SELECT with function ─────────────────────────────
SELECT id, name, fn_customer_total(id) AS total_paid
FROM customers;

-- ── INDEX ─────────────────────────────────────────────────
CREATE INDEX idx_orders_status ON orders(status);

-- ── TRANSACTION ───────────────────────────────────────────
START TRANSACTION;
INSERT INTO customers (name, email) VALUES ('Dave Test', 'dave@example.com');
ROLLBACK;

-- ── GRANT / permissions example (may need root) ───────────
-- GRANT SELECT ON test_schema.* TO 'appuser'@'%';
