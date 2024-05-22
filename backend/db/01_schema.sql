CREATE TABLE `users` (
  `id` INT PRIMARY KEY AUTO_INCREMENT,
  `username` VARCHAR(255),
  `password` VARCHAR(255),
  `role` VARCHAR(255),
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE `settings` (
  `id` INT PRIMARY KEY AUTO_INCREMENT,
  `name` VARCHAR(255),
  `description` VARCHAR(255)
);

CREATE TABLE `user_settings` (
  `id` INT PRIMARY KEY AUTO_INCREMENT,
  `user_id` INT,
  `setting_id` INT,
  `value` VARCHAR(255),
  FOREIGN KEY (`user_id`) REFERENCES `users` (`id`),
  FOREIGN KEY (`setting_id`) REFERENCES `settings` (`id`)
);

CREATE TABLE `achievements` (
  `id` INT PRIMARY KEY,
  `title` VARCHAR(255),
  `description` VARCHAR(255),
  `starting_value` INT,
  `target_value` INT
);

CREATE TABLE `user_achievements` (
  `id` INT PRIMARY KEY AUTO_INCREMENT,
  `user_id` INT,
  `achievement_id` INT,
  `value` INT,
  `is_finished` BOOLEAN DEFAULT FALSE,
  FOREIGN KEY (`user_id`) REFERENCES `users` (`id`),
  FOREIGN KEY (`achievement_id`) REFERENCES `achievements` (`id`)
);

DELIMITER //
CREATE TRIGGER ACHIEVEMENTS_CHECK AFTER UPDATE ON `user_achievements`
FOR EACH ROW BEGIN
    DECLARE x INT;
    SET x = (SELECT `target_value` FROM `achievements` WHERE `id` = NEW.`achievement_id`);
    IF NEW.`value` >= x AND NEW.`value` <> OLD.`value` AND NOT OLD.`is_finished` THEN
        UPDATE `user_achievements` SET `is_finished` = TRUE WHERE `id` = NEW.`id`; 
    END IF;
END;
//
DELIMITER ;
