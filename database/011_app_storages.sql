-- Table: public.app_storages

-- DROP TABLE IF EXISTS public.app_storages;

CREATE TABLE IF NOT EXISTS public.app_storages
(
    id bigint NOT NULL DEFAULT snow_flake_id(),
    alias_name character varying(32) COLLATE pg_catalog."default" NOT NULL,
    root_folder character varying(128) COLLATE pg_catalog."default" NOT NULL,
    readonly boolean NOT NULL DEFAULT true,
    roles character varying(64)[] COLLATE pg_catalog."default",
    CONSTRAINT pk_app_storages PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.app_storages
    OWNER to postgres;

COMMENT ON TABLE public.app_storages
    IS '应用存储';

COMMENT ON COLUMN public.app_storages.id
    IS '存储ID';

COMMENT ON COLUMN public.app_storages.alias_name
    IS '存储别名';

COMMENT ON COLUMN public.app_storages.root_folder
    IS '存储根路径';

COMMENT ON COLUMN public.app_storages.readonly
    IS '是否只读';

COMMENT ON COLUMN public.app_storages.roles
    IS '可访问此存储的角色';
