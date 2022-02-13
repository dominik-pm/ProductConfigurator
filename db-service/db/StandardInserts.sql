use product_configurator;

INSERT IGNORE INTO account(id, email, username)
VALUES (1, 'test@user.com', 'sqrt3'),
       (2, 'configurator-admin@test-fuchs.com', 'andifined'),
       (3, 'testuser@test-fuchs.com', 'testfuchs');

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