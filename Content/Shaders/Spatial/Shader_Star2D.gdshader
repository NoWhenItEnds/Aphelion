shader_type spatial;

uniform vec3 colour : source_color = vec3(1.0, 1.0, 1.0);
uniform int points : hint_range(1,20, 1) = 4;
uniform float thickness: hint_range(1.0, 20.0, 1.0) = 10.0;
uniform float relative_size : hint_range(0.0, 1.0, 0.1) = 1.0;
uniform float rotation_speed: hint_range(-10.0, 10.0, 0.1) = 1.0;

// https://godotshaders.com/snippet/rotate/
vec2 rotateUV(vec2 uv, vec2 pivot, float angle)
{
    mat2 rotation = mat2(vec2(sin(angle), -cos(angle)),
                        vec2(cos(angle), sin(angle)));

    uv -= pivot;
    uv = uv * rotation;
    uv += pivot;
    return uv;
}

void fragment() {
	vec2 uv = rotateUV(UV, vec2(0.5), TIME * rotation_speed) - vec2(0.5);
	float angle = atan(uv.y, uv.x);
	float r = sqrt(pow(uv.x, 2) + pow(uv.y, 2));
	float indentness = 0.3 * r * thickness;
	float b =  (r + abs(sin(angle * float(points) * 0.5)) * indentness);
	ALBEDO = colour;
	EMISSION = colour;
	ALPHA = (1.0 - step(0.0, b - (relative_size * 0.5))) * (1.0 + sin(TIME * rotation_speed));
}

// Billboard effect.
void vertex() {
    mat4 modified_model_view = VIEW_MATRIX * mat4(
        INV_VIEW_MATRIX[0],
        INV_VIEW_MATRIX[1],
        INV_VIEW_MATRIX[2],
        MODEL_MATRIX[3]
    );
    MODELVIEW_MATRIX = modified_model_view;
}