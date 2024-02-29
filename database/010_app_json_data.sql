-- Table: public.app_json_data

-- DROP TABLE IF EXISTS public.app_json_data;

CREATE TABLE IF NOT EXISTS public.app_json_data
(
    id bigint NOT NULL,
    value jsonb NOT NULL,
    CONSTRAINT pk_app_json_data PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.app_json_data
    OWNER to postgres;

COMMENT ON TABLE public.app_json_data
    IS 'JSON 数据';

COMMENT ON COLUMN public.app_json_data.id
    IS 'JSON 数据ID';

COMMENT ON COLUMN public.app_json_data.value
    IS 'JSON 值';
