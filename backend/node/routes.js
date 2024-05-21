const express = require('express');
const router = express.Router();
const mysql = require('mysql');
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');

// Database connection
const pool = mysql.createPool({
  connectionLimit: 10,
  host: process.env.DB_HOST,
  user: process.env.DB_USER,
  password: process.env.DB_PASSWORD,
  database: process.env.DB_NAME
});

const verifyToken = (req, res, next) => {
  const token = req.headers['x-access-token'];
  if (!token) {
    return res.status(403).send({ auth: false, message: 'No token provided.' });
  }

  jwt.verify(token, 'your_secret_key', (err, decoded) => {
    if (err) {
      return res.status(500).send({ auth: false, message: 'Failed to authenticate token.' });
    }

    // Store the user ID in request for further use
    req.userId = decoded.id;
    next();
  });
};

// Register user
router.post('/register', (req, res) => {
    const { username, password } = req.body;
    pool.query('SELECT * FROM users WHERE username = ?', [username], (error, results) => {
      if (error) {
        console.error('Database error:', error);
        return res.status(500).send('Database error');
      }
  
      if (results.length > 0) {
        return res.status(409).send({ message: 'Username already exists!' });
      }
  
      const hashedPassword = bcrypt.hashSync(password, 8);
      pool.query('INSERT INTO users (username, password) VALUES (?, ?)', [username, hashedPassword], (error, results) => {
        if (error) {
          console.error('Database error:', error);
          return res.status(500).send('Database error');
        }
        res.status(201).send({ message: 'User registered!', userId: results.insertId });
      });
    });
  });

// Login user
router.post('/login', (req, res) => {
  const { username, password } = req.body;

  pool.query('SELECT * FROM users WHERE username = ?', [username], (error, results) => {
    if (error) throw error;
    if (results.length > 0 && bcrypt.compareSync(password, results[0].password)) {
      const token = jwt.sign({ id: results[0].id }, 'your_secret_key', { expiresIn: 86400 });
      res.send({ auth: true, token });
    } else {
      res.status(401).send({ auth: false, token: null });
    }
  });
});

// Get achievements for authenticated user
router.get('/achievements', verifyToken, (req, res) => {
    pool.query('SELECT a.* FROM achievements a JOIN user_achievements ua ON a.id = ua.achievement_id WHERE ua.user_id = ?', [req.userId], (error, results) => {
      if (error) {
        console.error('Database error:', error);
        return res.status(500).send('Database error');
      }
      res.status(200).send(results);
    });
  });

// Update user achievements
router.put('/achievements', verifyToken, (req, res) => {
    const { achievementId, value } = req.body;
    const userId = req.userId;

    // First, try to update the achievement if it exists
    const updateQuery = 'UPDATE user_achievements SET value = ? WHERE user_id = ? AND achievement_id = ?';

    pool.query(updateQuery, [value, userId, achievementId], (error, results) => {
      if (error) {
        console.error('Database error:', error);
        return res.status(500).send('Database error');
      }
      if (results.affectedRows === 0) {
        // If no rows were affected, it means no matching achievement was found, insert new one
        const insertQuery = 'INSERT INTO user_achievements (user_id, achievement_id, value) VALUES (?, ?, ?)';
        pool.query(insertQuery, [userId, achievementId, value], (error, results) => {
          if (error) {
            console.error('Database error on insert:', error);
            return res.status(500).send('Database error on insert');
          }
          return res.status(201).send({ message: 'Achievement record created successfully!' });
        });
      } else {
        res.status(200).send({ message: 'Achievement updated successfully!' });
      }
    });
});

// Get all achievements
router.get('/all-achievements', (req, res) => {
  pool.query('SELECT * FROM achievements', (error, results) => {
      if (error) {
          console.error('Database error:', error);
          return res.status(500).send({ message: 'Error retrieving achievements from the database.' });
      }
      res.status(200).json(results);
  });
});

module.exports = router;
