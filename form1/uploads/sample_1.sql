-- Sample SQL file for database initialization
-- This file can be used to test the SQL File Executor

-- Create tables
CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS orders (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    amount DECIMAL(10, 2),
    order_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
);

-- Insert sample data
INSERT INTO users (name, email) VALUES ('John Doe', 'john@example.com');
INSERT INTO users (name, email) VALUES ('Jane Smith', 'jane@example.com');
INSERT INTO users (name, email) VALUES ('Bob Johnson', 'bob@example.com');

-- Insert orders
INSERT INTO orders (user_id, amount) VALUES (1, 150.00);
INSERT INTO orders (user_id, amount) VALUES (2, 200.50);
INSERT INTO orders (user_id, amount) VALUES (1, 75.25);

-- Create a stored procedure
DELIMITER //
CREATE PROCEDURE IF NOT EXISTS GetUserOrders(IN userId INT)
BEGIN
    SELECT o.id, o.amount, o.order_date 
    FROM orders o 
    WHERE o.user_id = userId;
END//
DELIMITER ;

-- Create another procedure for user count
DELIMITER //
CREATE PROCEDURE IF NOT EXISTS GetUserCount()
BEGIN
    SELECT COUNT(*) as total_users FROM users;
END//
DELIMITER ;

-- Create a view
CREATE OR REPLACE VIEW user_order_summary AS
SELECT 
    u.id,
    u.name,
    u.email,
    COUNT(o.id) as total_orders,
    COALESCE(SUM(o.amount), 0) as total_spent
FROM users u
LEFT JOIN orders o ON u.id = o.user_id
GROUP BY u.id, u.name, u.email;
