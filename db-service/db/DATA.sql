use product_configurator;

INSERT IGNORE INTO e_product_category(category)
VALUES ('CAR'),
       ('COLOR'),
       ('ENGINE'),
       ('PANORAMA_ROOF');

INSERT IGNORE INTO products (product_number, price, category, buyable)
VALUES ('Golf', 10000, 'CAR', 1),
       ('BLUE', 0, 'COLOR', 0),
       ('YELLOW', 200, 'COLOR', 0),
       ('GREEN', 500, 'COLOR', 0),
       ('D150', 8000, 'ENGINE', 0),
       ('D250', 11000, 'ENGINE', 0),
       ('P220', 9000, 'ENGINE', 0),
       ('P450', 16000, 'ENGINE', 0),
       ('PANORAMASMALL', 0, 'PANORAMA_ROOF', 0),
       ('PANORAMALARGE', 500, 'PANORAMA_ROOF', 0),
       ('PANORAMAROOF', 2000, 'PANORAMA_ROOF', 0),
       ('DIESEL', 0, 'ENGINE', 0),
       ('PETROL', 0, 'ENGINE', 0);

INSERT IGNORE INTO e_languages (language)
VALUES ('ENGLISH'),
       ('GERMAN');

INSERT IGNORE INTO product_has_language (product_number, language, name, description)
VALUES ('Golf', 'ENGLISH', 'Golf', 'its a car, to drive from A to B'),
       ('Blue', 'ENGLISH', 'Blue', 'The color blue!'),
       ('YELLOW', 'ENGLISH', 'Yellow', 'The color yellow!'),
       ('GREEN', 'ENGLISH', 'Green', 'The color green!'),
       ('D150', 'ENGLISH', '150PS Diesel', 'A Motor!'),
       ('D250', 'ENGLISH', '250PS Diesel', 'A Motor!'),
       ('P220', 'ENGLISH', '220PS Petrol', 'A Motor!'),
       ('P450', 'ENGLISH', '450PS Petrol', 'A Motor!'),
       ('PANORAMASMALL', 'ENGLISH', 'Small Roof', 'a small glass roof'),
       ('PANORAMALARGE', 'ENGLISH', 'Big Roof', 'a large glass roof for an amazing open feeling'),
       ('DIESEL', 'ENGLISH', 'Diesel Motor', 'A Diesel Motor'),
       ('PETROL', 'ENGLISH', 'Petrol Motor', 'A Petrol Motor'),
       ('PANORAMAROOF', 'ENGLISH', 'Panorama Roof', 'a glass roof'),

       ('Golf', 'GERMAN', 'Golf', 'es ist ein auto zum von A nach B fahren'),
       ('Blue', 'GERMAN', 'Blau', 'Die Farbe blau!'),
       ('YELLOW', 'GERMAN', 'Gelb', 'Die Farbe gelb!'),
       ('GREEN', 'GERMAN', 'Grün', 'Die Farbe grün!'),
       ('D150', 'GERMAN', '150PS Diesel', 'Ein Motor!'),
       ('D250', 'GERMAN', '250PS Diesel', 'Ein Motor!'),
       ('P220', 'GERMAN', '220PS Petrol', 'Ein Motor!'),
       ('P450', 'GERMAN', '450PS Petrol', 'Ein Motor!'),
       ('PANORAMASMALL', 'GERMAN', 'kleines Dach', 'ein kleines Glasfenster'),
       ('PANORAMALARGE', 'GERMAN', 'Großes Dach', 'ein großes Glasfenster'),
       ('DIESEL', 'GERMAN', 'Diesel Motor', 'Ein Diesel Motor'),
       ('PETROL', 'GERMAN', 'Benzin Motor', 'Ein Benzin Motor'),
       ('PANORAMAROOF', 'GERMAN', 'Panoramadach', 'ein Glasdach');

INSERT IGNORE INTO pictures (product_number, url)
VALUES ('Golf', 'https://cdn.motor1.com/images/mgl/G6VbA/s1/vw-golf-r-2021.jpg');

INSERT IGNORE INTO e_dependency_types (type)
VALUES ('PARENT'),
       ('CHILD'),
       ('EXCLUDING'),
       ('REQUIRED'),
       ('COMPATIBLE');

INSERT IGNORE INTO products_has_products (base_product, dependency_type, option_product)
VALUES ('D150', 'REQUIRED', 'DIESEl'),
       ('D250', 'REQUIRED', 'DIESEL'),
       ('P220', 'REQUIRED', 'PETROl'),
       ('P450', 'REQUIRED', 'PETROL'),
       ('PANORAMASMALL', 'REQUIRED', 'PANORAMAROOF'),
       ('PANORAMALARGE', 'REQUIRED', 'PANORAMAROOF'),

       ('PANORAMAROOF', 'EXCLUDING', 'PETROL');

INSERT IGNORE INTO e_option_types (type)
VALUES ('MULTI_SELECT'),
       ('SINGLE_SELECT'),
       ('PARENT');

INSERT IGNORE INTO option_fields (id, type, required)
VALUES (1, 'SINGLE_SELECT', 1), -- Color
       (2, 'SINGLE_SELECT', 1), -- Motor Type
       (3, 'SINGLE_SELECT', 1), -- Motor Group
       (4, 'SINGLE_SELECT', 0), -- Panorama Group
       (5, 'SINGLE_SELECT', 1), -- Panoramatype Group

       (6, 'PARENT', 0),        -- Exterior
       (7, 'PARENT', 0),        -- Motor Section
       (8, 'PARENT', 0); -- Panorama Section

INSERT IGNORE INTO products_has_option_fields (option_fields, product_number, dependency_type)
VALUES (1, 'BLUE', 'CHILD'),          -- Color
       (1, 'YELLOW', 'CHILD'),
       (1, 'GREEN', 'CHILD'),
       (2, 'DIESEL', 'CHILD'),        -- Motor Type
       (2, 'PETROL', 'CHILD'),
       (3, 'D150', 'CHILD'),          -- Motor Group
       (3, 'D250', 'CHILD'),
       (3, 'P220', 'CHILD'),
       (3, 'P450', 'CHILD'),
       (4, 'PANORAMAROOF', 'CHILD'),  -- Panorama Group
       (5, 'PANORAMASMALL', 'CHILD'), -- Panorama Type
       (5, 'PANORAMALARGE', 'CHILD'),

       -- The option-fields for a car
       (6, 'Golf', 'PARENT'),
       (7, 'Golf', 'PARENT'),
       (8, 'Golf', 'PARENT');

INSERT IGNORE INTO option_field_has_language (option_field_id, language, name, description)
VALUES (1, 'ENGLISH', 'Color', 'the exterior color of the car'),
       (2, 'ENGLISH', 'Motor Type', 'the motor of your car'),
       (3, 'ENGLISH', 'Motor', 'how powerful'),
       (4, 'ENGLISH', 'Panoramic Roof', 'a glass roof for an open feeling'),
       (5, 'ENGLISH', 'Panoramic Roof type', 'size of your panorama roof'),
       (6, 'ENGLISH', 'EXTERIOR', 'exterior'),
       (7, 'ENGLISH', 'MOTOR_SECTION', 'Motor'),
       (8, 'ENGLISH', 'PANORAMA_SECTION', 'Panorama'),

       (1, 'GERMAN', 'Farbe', 'die Außenfarbe des Autos'),
       (2, 'GERMAN', 'Motor Typ', 'der Motor des Autos'),
       (3, 'GERMAN', 'Motor', 'wie stark'),
       (4, 'GERMAN', 'Panorama Dach', 'ein Glasdach für ein offenes Gefühl'),
       (5, 'GERMAN', 'Panorama Dach Typ', 'Größe des Glasdaches'),
       (6, 'GERMAN', 'EXTERIOR', 'exterior'),
       (7, 'GERMAN', 'MOTOR_BEREICH', 'Motor'),
       (8, 'GERMAN', 'PANORAMA_BEREICH', 'Panorama');

INSERT IGNORE INTO option_fields_has_option_fields (base, option_field, dependency_type)
VALUES (6, 1, 'CHILD'),
       (7, 2, 'CHILD'),
       (7, 3, 'CHILD'),
       (8, 4, 'CHILD'),
       (8, 5, 'CHILD');

INSERT IGNORE INTO configurations (id, product_number)
VALUES (1, 'Golf'),
       (2, 'Golf');

INSERT IGNORE INTO bookings (Customer, config_id)
VALUES (1, 1),
       (2, 1),
       (1, 2);

INSERT IGNORE INTO configuration_has_option_fields (config_id, option_field_id, parent_config_id, parent_option_field_id)
VALUES (1, 6, null, null),
       (1, 7, null, null),
       (1, 8, null, null),

       (1, 1, 1, 6),
       (1, 2, 1, 7),
       (1, 3, 1, 7),
       (1, 4, 1, 8),
       (1, 5, 1, 8),
       --
       (2, 6, null, null),
       (2, 7, null, null),
       (2, 8, null, null),

       (2, 1, 2, 6),
       (2, 2, 2, 7),
       (2, 3, 2, 7),
       (2, 4, 2, 8),
       (2, 5, 2, 8);

INSERT IGNORE INTO configuration_has_selected_options (config_id, option_field_id, product_number)
VALUES (1, 1, 'BLUE'),
       (1, 2, 'PETROL'),
       (1, 3, 'P220'),

       (2, 1, 'GREEN'),
       (2, 2, 'DIESEL'),
       (2, 3, 'D250'),
       (2, 4, 'PANORAMAROOF'),
       (2, 5, 'PANORAMALARGE');
