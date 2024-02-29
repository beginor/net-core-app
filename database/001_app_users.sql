-- Table: public.app_users

-- DROP TABLE IF EXISTS public.app_users;

CREATE TABLE IF NOT EXISTS public.app_users
(
    id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    create_time timestamp without time zone NOT NULL DEFAULT now(),
    last_login timestamp without time zone,
    login_count integer DEFAULT 0,
    organize_unit_id bigint NOT NULL DEFAULT 0,
    display_name character varying(64) COLLATE pg_catalog."default",
    CONSTRAINT pk_app_users PRIMARY KEY (id),
    CONSTRAINT fk_app_users_organize_unit FOREIGN KEY (organize_unit_id)
        REFERENCES public.app_organize_units (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
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

COMMENT ON COLUMN public.app_users.create_time
    IS '创建时间';

COMMENT ON COLUMN public.app_users.last_login
    IS '最近登录时间';

COMMENT ON COLUMN public.app_users.login_count
    IS '登录次数';

COMMENT ON COLUMN public.app_users.organize_unit_id
    IS '组织单元ID';

COMMENT ON COLUMN public.app_users.display_name
    IS '显示名称';
