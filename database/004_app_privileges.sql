-- Table: public.app_privileges

-- DROP TABLE IF EXISTS public.app_privileges;

CREATE TABLE IF NOT EXISTS public.app_privileges
(
    id bigint NOT NULL DEFAULT snow_flake_id(),
    module character varying(32) COLLATE pg_catalog."default" NOT NULL,
    name character varying(64) COLLATE pg_catalog."default" NOT NULL,
    description character varying(128) COLLATE pg_catalog."default",
    is_required boolean NOT NULL DEFAULT false,
    CONSTRAINT pk_app_privileges PRIMARY KEY (id),
    CONSTRAINT uk_app_privileges_name UNIQUE (name)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.app_privileges
    OWNER to postgres;

COMMENT ON TABLE public.app_privileges
    IS '系统权限';

COMMENT ON COLUMN public.app_privileges.id
    IS '权限ID';

COMMENT ON COLUMN public.app_privileges.module
    IS '权限模块';

COMMENT ON COLUMN public.app_privileges.name
    IS '权限名称( identity 的策略名称)';

COMMENT ON COLUMN public.app_privileges.description
    IS '权限描述';

COMMENT ON COLUMN public.app_privileges.is_required
    IS '是否必须。 与代码中的 authorize 标记对应的权限为必须的权限， 否则为可选的。';
