-- Table: public.app_organize_units

-- DROP TABLE IF EXISTS public.app_organize_units;

CREATE TABLE IF NOT EXISTS public.app_organize_units
(
    id bigint NOT NULL DEFAULT snow_flake_id(),
    parent_id bigint,
    code character varying(512) COLLATE pg_catalog."default",
    name character varying(32) COLLATE pg_catalog."default" NOT NULL,
    description character varying(128) COLLATE pg_catalog."default",
    sequence real NOT NULL,
    creator_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    created_at timestamp without time zone NOT NULL,
    updater_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    updated_at timestamp without time zone NOT NULL,
    is_deleted boolean NOT NULL,
    CONSTRAINT pk_app_organize_unit PRIMARY KEY (id),
    CONSTRAINT fk_app_organize_unit_creator FOREIGN KEY (creator_id)
        REFERENCES public.app_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT fk_app_organize_unit_updator FOREIGN KEY (updater_id)
        REFERENCES public.app_users (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.app_organize_units
    OWNER to postgres;

COMMENT ON TABLE public.app_organize_units
    IS '组织单元';

COMMENT ON COLUMN public.app_organize_units.id
    IS '组织单元ID';

COMMENT ON COLUMN public.app_organize_units.parent_id
    IS '上级组织单元 ID';

COMMENT ON COLUMN public.app_organize_units.code
    IS '组织单元编码';

COMMENT ON COLUMN public.app_organize_units.name
    IS '组织单元名称';

COMMENT ON COLUMN public.app_organize_units.description
    IS '组织单元说明';

COMMENT ON COLUMN public.app_organize_units.sequence
    IS '组织机构排序';

COMMENT ON COLUMN public.app_organize_units.creator_id
    IS '创建者ID';

COMMENT ON COLUMN public.app_organize_units.created_at
    IS '创建时间';

COMMENT ON COLUMN public.app_organize_units.updater_id
    IS '更新者ID';

COMMENT ON COLUMN public.app_organize_units.updated_at
    IS '更新时间';

COMMENT ON COLUMN public.app_organize_units.is_deleted
    IS '是否删除';
