<script lang="ts">
	import { Badge } from '$lib/components/ui/badge/index.js';
	import HeartHandshake from '@lucide/svelte/icons/heart-handshake';
</script>

<div class="container mx-auto space-y-6 p-4 lg:p-6">
	<!-- Header de Boas-vindas -->
	<div class="space-y-4 text-center">

		<div class="space-y-2">
			<h1 class="text-3xl font-bold tracking-tight">Bem-vindo(a) de volta!</h1>
			<p class="text-muted-foreground text-lg">Pronto para cuidar dos seus pacientes hoje?</p>
		</div>

		<Badge variant="outline" class="text-sm">
			<HeartHandshake class="mr-2 h-4 w-4" />
			Profissional de Saúde
		</Badge>
		<Badge variant="outline" class="text-sm">Plano Grátis</Badge>
	</div>
	<!--
	<Separator />
    -->
</div>
