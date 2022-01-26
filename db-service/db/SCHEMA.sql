-- MySQL Script generated by MySQL Workbench
-- Wed Jan 26 17:44:51 2022
-- Model: New Model    Version: 1.0
-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema Product_Configurator
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema Product_Configurator
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `Product_Configurator` DEFAULT CHARACTER SET utf8 ;
USE `Product_Configurator` ;

-- -----------------------------------------------------
-- Table `Product_Configurator`.`E_PRODUCT_CATEGORY`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`E_PRODUCT_CATEGORY` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`E_PRODUCT_CATEGORY` (
  `category` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`category`),
  UNIQUE INDEX `category_UNIQUE` (`category` ASC) VISIBLE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`PRODUCTS`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`PRODUCTS` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`PRODUCTS` (
  `product_number` VARCHAR(255) NOT NULL,
  `price` FLOAT(12,2) NOT NULL,
  `category` VARCHAR(255) NOT NULL,
  `buyable` TINYINT(1) NOT NULL,
  PRIMARY KEY (`product_number`),
  UNIQUE INDEX `product_number_UNIQUE` (`product_number` ASC) VISIBLE,
  INDEX `fk_PRODUCTS_E_PRODUCT_CATEGORY1_idx` (`category` ASC) VISIBLE,
  CONSTRAINT `fk_PRODUCTS_E_PRODUCT_CATEGORY1`
    FOREIGN KEY (`category`)
    REFERENCES `Product_Configurator`.`E_PRODUCT_CATEGORY` (`category`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`E_DEPENDENCY_TYPES`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`E_DEPENDENCY_TYPES` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`E_DEPENDENCY_TYPES` (
  `type` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`type`),
  UNIQUE INDEX `type_UNIQUE` (`type` ASC) VISIBLE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`PRODUCTS_has_PRODUCTS`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`PRODUCTS_has_PRODUCTS` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`PRODUCTS_has_PRODUCTS` (
  `base_product` VARCHAR(255) NOT NULL,
  `option_product` VARCHAR(255) NOT NULL,
  `dependency_type` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`base_product`, `option_product`),
  INDEX `fk_PRODUCTS_has_PRODUCTS_PRODUCTS1_idx` (`option_product` ASC) VISIBLE,
  INDEX `fk_PRODUCTS_has_PRODUCTS_PRODUCTS_idx` (`base_product` ASC) VISIBLE,
  INDEX `fk_PRODUCTS_has_PRODUCTS_E_DEPENDENCY_TYPE1_idx` (`dependency_type` ASC) VISIBLE,
  CONSTRAINT `fk_PRODUCTS_has_PRODUCTS_PRODUCTS`
    FOREIGN KEY (`base_product`)
    REFERENCES `Product_Configurator`.`PRODUCTS` (`product_number`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_PRODUCTS_has_PRODUCTS_PRODUCTS1`
    FOREIGN KEY (`option_product`)
    REFERENCES `Product_Configurator`.`PRODUCTS` (`product_number`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_PRODUCTS_has_PRODUCTS_E_DEPENDENCY_TYPE1`
    FOREIGN KEY (`dependency_type`)
    REFERENCES `Product_Configurator`.`E_DEPENDENCY_TYPES` (`type`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`E_OPTION_TYPES`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`E_OPTION_TYPES` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`E_OPTION_TYPES` (
  `type` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`type`),
  UNIQUE INDEX `TYPE_UNIQUE` (`type` ASC) VISIBLE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`OPTION_FIELDS`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`OPTION_FIELDS` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`OPTION_FIELDS` (
  `id` VARCHAR(255) NOT NULL,
  `type` VARCHAR(255) NOT NULL,
  `required` TINYINT(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC) VISIBLE,
  INDEX `fk_OPTION_FIELD_E_OPTION_TYPES1_idx` (`type` ASC) VISIBLE,
  CONSTRAINT `fk_OPTION_FIELD_E_OPTION_TYPES1`
    FOREIGN KEY (`type`)
    REFERENCES `Product_Configurator`.`E_OPTION_TYPES` (`type`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`E_LANGUAGES`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`E_LANGUAGES` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`E_LANGUAGES` (
  `language` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`language`),
  UNIQUE INDEX `name_UNIQUE` (`language` ASC) VISIBLE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`CONFIGURATIONS`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`CONFIGURATIONS` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`CONFIGURATIONS` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `product_number` VARCHAR(255) NOT NULL,
  `Customer` INT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_BOOKINGS_PRODUCTS1_idx` (`product_number` ASC) VISIBLE,
  CONSTRAINT `fk_BOOKINGS_PRODUCTS1`
    FOREIGN KEY (`product_number`)
    REFERENCES `Product_Configurator`.`PRODUCTS` (`product_number`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`CONFIGURATION_has_OPTION_FIELDS`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`CONFIGURATION_has_OPTION_FIELDS` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`CONFIGURATION_has_OPTION_FIELDS` (
  `config_id` INT NOT NULL,
  `option_field_id` VARCHAR(255) NOT NULL,
  `parent_config_id` INT NULL,
  `parent_option_field_id` VARCHAR(255) NULL,
  PRIMARY KEY (`config_id`, `option_field_id`),
  INDEX `fk_PRODUCTS_has_OPTION_FIELD_has_BOOKINGS_BOOKINGS1_idx` (`config_id` ASC) VISIBLE,
  INDEX `fk_PRODUCTS_has_OPTION_FIELD_has_BOOKINGS_OPTION_FIELD1_idx` (`option_field_id` ASC) VISIBLE,
  INDEX `fk_CONFIGURATION_has_OPTION_FIELDS_CONFIGURATION_has_OPTION_idx` (`parent_config_id` ASC, `parent_option_field_id` ASC) VISIBLE,
  CONSTRAINT `fk_PRODUCTS_has_OPTION_FIELD_has_BOOKINGS_BOOKINGS1`
    FOREIGN KEY (`config_id`)
    REFERENCES `Product_Configurator`.`CONFIGURATIONS` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_PRODUCTS_has_OPTION_FIELD_has_BOOKINGS_OPTION_FIELD1`
    FOREIGN KEY (`option_field_id`)
    REFERENCES `Product_Configurator`.`OPTION_FIELDS` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_CONFIGURATION_has_OPTION_FIELDS_CONFIGURATION_has_OPTION_F1`
    FOREIGN KEY (`parent_config_id` , `parent_option_field_id`)
    REFERENCES `Product_Configurator`.`CONFIGURATION_has_OPTION_FIELDS` (`config_id` , `option_field_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`CONFIGURATION_has_SELECTED_OPTIONS`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`CONFIGURATION_has_SELECTED_OPTIONS` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`CONFIGURATION_has_SELECTED_OPTIONS` (
  `config_id` INT NOT NULL,
  `option_field_id` VARCHAR(255) NOT NULL,
  `product_number` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`config_id`, `option_field_id`, `product_number`),
  INDEX `fk_BOOKINGS_has_OPTION_FIELDS_has_PRODUCTS_PRODUCTS1_idx` (`product_number` ASC) VISIBLE,
  INDEX `fk_BOOKINGS_has_OPTION_FIELDS_has_PRODUCTS_BOOKINGS_has_OPT_idx` (`config_id` ASC, `option_field_id` ASC) VISIBLE,
  CONSTRAINT `fk_BOOKINGS_has_OPTION_FIELDS_has_PRODUCTS_BOOKINGS_has_OPTIO1`
    FOREIGN KEY (`config_id` , `option_field_id`)
    REFERENCES `Product_Configurator`.`CONFIGURATION_has_OPTION_FIELDS` (`config_id` , `option_field_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_BOOKINGS_has_OPTION_FIELDS_has_PRODUCTS_PRODUCTS1`
    FOREIGN KEY (`product_number`)
    REFERENCES `Product_Configurator`.`PRODUCTS` (`product_number`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`PRODUCT_has_LANGUAGE`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`PRODUCT_has_LANGUAGE` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`PRODUCT_has_LANGUAGE` (
  `product_number` VARCHAR(255) NOT NULL,
  `language` VARCHAR(45) NOT NULL,
  `name` VARCHAR(255) NOT NULL,
  `description` TEXT NOT NULL,
  PRIMARY KEY (`product_number`, `language`),
  INDEX `fk_PRODUCTS_has_ELANGUAGES_ELANGUAGES1_idx` (`language` ASC) VISIBLE,
  INDEX `fk_PRODUCTS_has_ELANGUAGES_PRODUCTS1_idx` (`product_number` ASC) VISIBLE,
  CONSTRAINT `fk_PRODUCTS_has_ELANGUAGES_PRODUCTS1`
    FOREIGN KEY (`product_number`)
    REFERENCES `Product_Configurator`.`PRODUCTS` (`product_number`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_PRODUCTS_has_ELANGUAGES_ELANGUAGES1`
    FOREIGN KEY (`language`)
    REFERENCES `Product_Configurator`.`E_LANGUAGES` (`language`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`OPTION_FIELD_has_LANGUAGE`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`OPTION_FIELD_has_LANGUAGE` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`OPTION_FIELD_has_LANGUAGE` (
  `option_field_id` VARCHAR(255) NOT NULL,
  `language` VARCHAR(45) NOT NULL,
  `name` VARCHAR(255) NOT NULL,
  `description` TEXT NOT NULL,
  PRIMARY KEY (`option_field_id`, `language`),
  INDEX `fk_ELANGUAGES_has_OPTION_FIELDS_OPTION_FIELDS1_idx` (`option_field_id` ASC) VISIBLE,
  INDEX `fk_ELANGUAGES_has_OPTION_FIELDS_ELANGUAGES1_idx` (`language` ASC) VISIBLE,
  CONSTRAINT `fk_ELANGUAGES_has_OPTION_FIELDS_ELANGUAGES1`
    FOREIGN KEY (`language`)
    REFERENCES `Product_Configurator`.`E_LANGUAGES` (`language`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_ELANGUAGES_has_OPTION_FIELDS_OPTION_FIELDS1`
    FOREIGN KEY (`option_field_id`)
    REFERENCES `Product_Configurator`.`OPTION_FIELDS` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`OPTION_FIELDS_has_OPTION_FIELDS`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`OPTION_FIELDS_has_OPTION_FIELDS` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`OPTION_FIELDS_has_OPTION_FIELDS` (
  `base` VARCHAR(255) NOT NULL,
  `option_field` VARCHAR(255) NOT NULL,
  `dependency_type` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`base`, `option_field`),
  INDEX `fk_OPTION_FIELDS_has_OPTION_FIELDS_OPTION_FIELDS2_idx` (`option_field` ASC) VISIBLE,
  INDEX `fk_OPTION_FIELDS_has_OPTION_FIELDS_OPTION_FIELDS1_idx` (`base` ASC) VISIBLE,
  INDEX `fk_OPTION_FIELDS_has_OPTION_FIELDS_E_DEPENDENCY_TYPES1_idx` (`dependency_type` ASC) VISIBLE,
  CONSTRAINT `fk_OPTION_FIELDS_has_OPTION_FIELDS_OPTION_FIELDS1`
    FOREIGN KEY (`base`)
    REFERENCES `Product_Configurator`.`OPTION_FIELDS` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_OPTION_FIELDS_has_OPTION_FIELDS_OPTION_FIELDS2`
    FOREIGN KEY (`option_field`)
    REFERENCES `Product_Configurator`.`OPTION_FIELDS` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_OPTION_FIELDS_has_OPTION_FIELDS_E_DEPENDENCY_TYPES1`
    FOREIGN KEY (`dependency_type`)
    REFERENCES `Product_Configurator`.`E_DEPENDENCY_TYPES` (`type`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`BOOKINGS`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`BOOKINGS` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`BOOKINGS` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `Customer` INT NOT NULL,
  `config_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC) VISIBLE,
  INDEX `fk_BOOKINGS_CONFIGURATIONS1_idx` (`config_id` ASC) VISIBLE,
  CONSTRAINT `fk_BOOKINGS_CONFIGURATIONS1`
    FOREIGN KEY (`config_id`)
    REFERENCES `Product_Configurator`.`CONFIGURATIONS` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`PICTURES`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`PICTURES` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`PICTURES` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `product_number` VARCHAR(255) NOT NULL,
  `url` TEXT NOT NULL,
  INDEX `fk_Picture_PRODUCTS1_idx` (`product_number` ASC) VISIBLE,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC) VISIBLE,
  CONSTRAINT `fk_Picture_PRODUCTS1`
    FOREIGN KEY (`product_number`)
    REFERENCES `Product_Configurator`.`PRODUCTS` (`product_number`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`PRODUCTS_has_OPTION_FIELDS`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`PRODUCTS_has_OPTION_FIELDS` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`PRODUCTS_has_OPTION_FIELDS` (
  `product_number` VARCHAR(255) NOT NULL,
  `option_fields` VARCHAR(255) NOT NULL,
  `dependency_type` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`product_number`, `option_fields`),
  INDEX `fk_PRODUCTS_has_OPTION_FIELDS_OPTION_FIELDS1_idx` (`option_fields` ASC) VISIBLE,
  INDEX `fk_PRODUCTS_has_OPTION_FIELDS_PRODUCTS1_idx` (`product_number` ASC) VISIBLE,
  INDEX `fk_PRODUCTS_has_OPTION_FIELDS_E_DEPENDENCY_TYPES1_idx` (`dependency_type` ASC) VISIBLE,
  CONSTRAINT `fk_PRODUCTS_has_OPTION_FIELDS_PRODUCTS1`
    FOREIGN KEY (`product_number`)
    REFERENCES `Product_Configurator`.`PRODUCTS` (`product_number`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_PRODUCTS_has_OPTION_FIELDS_OPTION_FIELDS1`
    FOREIGN KEY (`option_fields`)
    REFERENCES `Product_Configurator`.`OPTION_FIELDS` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_PRODUCTS_has_OPTION_FIELDS_E_DEPENDENCY_TYPES1`
    FOREIGN KEY (`dependency_type`)
    REFERENCES `Product_Configurator`.`E_DEPENDENCY_TYPES` (`type`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Product_Configurator`.`CONFIGURATIONS_has_LANGUAGE`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product_Configurator`.`CONFIGURATIONS_has_LANGUAGE` ;

CREATE TABLE IF NOT EXISTS `Product_Configurator`.`CONFIGURATIONS_has_LANGUAGE` (
  `configuration` INT NOT NULL,
  `language` VARCHAR(45) NOT NULL,
  `name` VARCHAR(255) NOT NULL,
  `description` TEXT NOT NULL,
  PRIMARY KEY (`configuration`, `language`),
  INDEX `fk_CONFIGURATIONS_has_E_LANGUAGES_E_LANGUAGES1_idx` (`language` ASC) VISIBLE,
  INDEX `fk_CONFIGURATIONS_has_E_LANGUAGES_CONFIGURATIONS1_idx` (`configuration` ASC) VISIBLE,
  CONSTRAINT `fk_CONFIGURATIONS_has_E_LANGUAGES_CONFIGURATIONS1`
    FOREIGN KEY (`configuration`)
    REFERENCES `Product_Configurator`.`CONFIGURATIONS` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_CONFIGURATIONS_has_E_LANGUAGES_E_LANGUAGES1`
    FOREIGN KEY (`language`)
    REFERENCES `Product_Configurator`.`E_LANGUAGES` (`language`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
