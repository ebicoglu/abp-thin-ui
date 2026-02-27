{{- define "multilayerabp.hosts.httpapi" -}}
{{- print "https://" (.Values.global.hosts.httpapi | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
