use product_configurator;

# DELETE

SET foreign_key_checks = 0;

DELETE FROM configuration_has_selected_options where 1 = 1;
DELETE FROM configuration_has_option_fields where 1 = 1;
DELETE FROM bookings where 1 = 1;
DELETE FROM configurations_has_language where 1 = 1;
DELETE FROM configurations where 1 = 1;
DELETE FROM option_fields_has_option_fields where 1 = 1;
DELETE FROM option_field_has_language where 1 = 1;
DELETE FROM products_has_option_fields where 1 = 1;
DELETE FROM option_fields where 1 = 1;
DELETE FROM e_option_types where 1 = 1;
DELETE FROM products_has_products where 1 = 1;
DELETE FROM e_dependency_types where 1 = 1;
DELETE FROM pictures where 1 = 1;
DELETE FROM product_has_language where 1 = 1;
DELETE FROM e_languages where 1 = 1;
DELETE FROM products where 1 = 1;
DELETE FROM account where 1 = 1;

SET foreign_key_checks = 1;

# INSERT

INSERT IGNORE INTO account(id, email, username, isAdmin)
VALUES (1, 'test@user.com', 'sqrt3', 1),
       (2, 'configurator-admin@test-fuchs.com', 'andifined', 0),
       (3, 'testuser@test-fuchs.com', 'testfuchs', 1);

INSERT IGNORE INTO e_languages (language)
VALUES ('en'),
       ('de');

INSERT IGNORE INTO e_dependency_types (type)
VALUES ('PARENT'),
       ('CHILD'),
       ('EXCLUDING'),
       ('REQUIRED'),
       ('COMPATIBLE');

INSERT IGNORE INTO e_option_types (type)
VALUES ('MULTI_SELECT'),
       ('SINGLE_SELECT'),
       ('PARENT');