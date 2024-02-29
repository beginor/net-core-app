-- Table: public.app_roles

-- DROP TABLE IF EXISTS public.app_roles;

CREATE TABLE IF NOT EXISTS public.app_roles
(
    id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    description character varying(256) COLLATE pg_catalog."default",
    is_default boolean DEFAULT false,
    is_anonymous boolean DEFAULT false,
    CONSTRAINT pk_app_roles PRIMARY KEY (id),
    CONSTRAINT fk_aspnet_roles_id FOREIGN KEY (id)
        REFERENCES public.aspnet_roles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.app_roles
    OWNER to postgres;

COMMENT ON TABLE public.app_roles
    IS '角色扩展信息';

COMMENT ON COLUMN public.app_roles.id
    IS '角色ID';

COMMENT ON COLUMN public.app_roles.description
    IS '角色描述';

COMMENT ON COLUMN public.app_roles.is_default
    IS '是否默认角色';

COMMENT ON COLUMN public.app_roles.is_anonymous
    IS '是否匿名角色';
