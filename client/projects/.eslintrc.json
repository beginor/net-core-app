{
  "extends": "../.eslintrc.json",
  "overrides": [
    {
      "files": ["*.ts"],
      "rules": {
        "quotes": ["warn", "single"],
        "no-console": ["warn", { "allow": ["error"] }],
        "no-debugger": ["warn"],
        "no-alert": ["warn"],
        "max-len": ["warn"],
        "brace-style": ["warn", "stroustrup", { "allowSingleLine": true }],
        "@typescript-eslint/ban-types": ["off"],
        "@typescript-eslint/explicit-member-accessibility": [
          "warn",
          { "overrides": { "constructors": "no-public" } }
        ],
        "@typescript-eslint/typedef": [
          "warn",
          {
            "parameter": true,
            "propertyDeclaration": true
          }
        ],
        "@typescript-eslint/no-implicit-any-catch": [
          "warn",
          { "allowExplicitAny": true }
        ],
        "@typescript-eslint/no-explicit-any": ["off"],
        "@typescript-eslint/no-unsafe-call": ["warn"],
        "@typescript-eslint/no-unsafe-assignment": ["warn"],
        "@typescript-eslint/restrict-template-expressions": ["warn"],
        "@typescript-eslint/no-unsafe-member-access": ["warn"],
        "@typescript-eslint/no-unsafe-return": ["warn"],
        "@typescript-eslint/no-empty-function": ["warn"],
        "@typescript-eslint/no-floating-promises": ["warn"],
        "@typescript-eslint/no-misused-promises": [
          "warn",
          {
            "checksVoidReturn": true,
            "checksConditionals": true
          }
        ],
        "@typescript-eslint/explicit-function-return-type": [
          "warn",
          {
            "allowExpressions": false,
            "allowTypedFunctionExpressions": true,
            "allowHigherOrderFunctions": true,
            "allowDirectConstAssertionInArrowFunctions": true,
            "allowConciseArrowFunctionExpressionsStartingWithVoid": true
          }
        ],
        "@typescript-eslint/explicit-module-boundary-types": [
          "warn"
        ]
      }
    }
  ]
}
