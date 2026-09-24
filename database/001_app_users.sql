-- Table: public.app_users

-- DROP TABLE IF EXISTS public.app_users;

CREATE TABLE IF NOT EXISTS public.app_users
(
    id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    created_at timestamp without time zone NOT NULL DEFAULT now(),
    last_login timestamp without time zone,
    login_count integer DEFAULT 0,
    organize_unit_id bigint NOT NULL DEFAULT 0,
    display_name character varying(64) COLLATE pg_catalog."default",
    surname character varying(32) COLLATE pg_catalog."default",
    given_name character varying(32) COLLATE pg_catalog."default",
    date_of_birth character varying(32) COLLATE pg_catalog."default",
    gender character varying(16) COLLATE pg_catalog."default",
    street_address character varying(256) COLLATE pg_catalog."default",
    CONSTRAINT pk_app_users PRIMARY KEY (id),
    CONSTRAINT fk_aspnet_users_id FOREIGN KEY (id)
        REFERENCES public.aspnet_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.app_users
    OWNER to postgres;

COMMENT ON TABLE public.app_users
    IS '用户扩展信息';

COMMENT ON COLUMN public.app_users.id
    IS '用户ID';

COMMENT ON COLUMN public.app_users.created_at
    IS '创建时间';

COMMENT ON COLUMN public.app_users.last_login
    IS '最近登录时间';

COMMENT ON COLUMN public.app_users.login_count
    IS '登录次数';

COMMENT ON COLUMN public.app_users.organize_unit_id
    IS '组织单元ID';

COMMENT ON COLUMN public.app_users.display_name
    IS '显示名称';

COMMENT ON COLUMN public.app_users.surname
    IS '姓';

COMMENT ON COLUMN public.app_users.given_name
    IS '名';

COMMENT ON COLUMN public.app_users.date_of_birth
    IS '出生日期';

COMMENT ON COLUMN public.app_users.gender
    IS '性别';

COMMENT ON COLUMN public.app_users.street_address
    IS '地址';
