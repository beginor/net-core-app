-- Add creator_id, created_at, updater_id, updated_at, is_deleted TO xxx table;
-- xxx is your table name, please replace it with your table name.

ALTER TABLE IF EXISTS public.xxx
    ADD COLUMN creator_id character varying(32) NOT NULL;

COMMENT ON COLUMN public.xxx.creator_id
    IS '创建者ID';

ALTER TABLE IF EXISTS public.xxx
    ADD COLUMN created_at timestamp without time zone NOT NULL DEFAULT now();

COMMENT ON COLUMN public.xxx.created_at
    IS '创建时间';

ALTER TABLE IF EXISTS public.xxx
    ADD COLUMN updater_id character varying(32) NOT NULL;

COMMENT ON COLUMN public.xxx.updater_id
    IS '更新者ID';

ALTER TABLE IF EXISTS public.xxx
    ADD COLUMN updated_at timestamp without time zone NOT NULL DEFAULT now();

COMMENT ON COLUMN public.xxx.updated_at
    IS '更新时间';

ALTER TABLE IF EXISTS public.xxx
    ADD COLUMN is_deleted boolean NOT NULL DEFAULT false;

COMMENT ON COLUMN public.xxx.is_deleted
    IS '是否删除';

ALTER TABLE IF EXISTS public.xxx
    ADD CONSTRAINT fk_xxx_creator FOREIGN KEY (creator_id)
    REFERENCES public.app_users (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;

ALTER TABLE IF EXISTS public.xxx
    ADD CONSTRAINT fk_xxx_updater FOREIGN KEY (updater_id)
    REFERENCES public.app_users (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;
