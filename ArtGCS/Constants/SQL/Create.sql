CREATE TABLE 'file' (
    'id'              INTEGER       PRIMARY KEY AUTOINCREMENT,
    'local_path'      TEXT          NOT NULL,
    'xxhash'          BLOB (8, 8)   NOT NULL,
    'first_save_time' TEXT (20, 20) NOT NULL
);

CREATE TABLE 'user' (
    'id'               INTEGER       PRIMARY KEY AUTOINCREMENT,
    'Name'             TEXT (128)    NOT NULL UNIQUE,
    'first_save_time'  TEXT (20, 20) NOT NULL,
    'last_update_time' TEXT (20, 20) NOT NULL
);


CREATE TABLE 'gallery' (
    'id'               INTEGER         PRIMARY KEY AUTOINCREMENT,
    'resource'         TEXT (64)       NOT NULL,
    'owner'            INTEGER         REFERENCES 'user' ('id') ON DELETE CASCADE
                                        NOT NULL,
    'nick_name'        TEXT (128)      NOT NULL,
    'creation_time'    TEXT (20, 20),
    'status'           TEXT (128, 128),
    'description'      TEXT,
    'icon_file'        INTEGER         REFERENCES 'file' ('id'),
    'first_save_time'  TEXT (20, 20)   NOT NULL,
    'last_update_time' TEXT (20, 20)   NOT NULL
);


CREATE TABLE 'submission' (
    'id'               INTEGER       PRIMARY KEY AUTOINCREMENT,
    'uri'              TEXT          UNIQUE
                                      NOT NULL,
    'file_uri'         TEXT          UNIQUE
                                      NOT NULL,
    'file'             INTEGER       REFERENCES 'file' ('id') ON DELETE CASCADE
                                      NOT NULL,
    'title'            TEXT,
    'description'      TEXT,
    'tags'             TEXT,
    'source_gallery'   INTEGER       REFERENCES 'gallery' ('id') ON DELETE CASCADE
                                      NOT NULL,
    'publication_time' TEXT (20)     NOT NULL,
    'first_save_time'  TEXT (20)     NOT NULL,
    'last_update_time' TEXT (20, 20) NOT NULL
);

CREATE TABLE 'changes' (
    'id'          INTEGER       PRIMARY KEY AUTOINCREMENT,
    'save_time'   TEXT (20, 20) NOT NULL,
    'change_time' TEXT (20, 20) NOT NULL,
    'table_name'  TEXT (16)     NOT NULL,
    'column_name' TEXT (16)     NOT NULL,
    'row_id'      INTEGER       NOT NULL,
    'old_data'    TEXT
);
