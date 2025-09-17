<script lang="ts">
	import { Button } from "$lib/components/ui/button/index.js";
	import { Separator } from "$lib/components/ui/separator/index.js";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";
	import { page } from "$app/state";

	// Mapeamento de rotas para títulos das páginas
	const pageTitles: Record<string, string> = {
		"/dashboard/healthcare-professional": "Dashboard",
		"/dashboard/healthcare-professional/appointments": "Agendamentos",
	};

	// Função para obter o título da página atual
	function getPageTitle(pathname: string): string {
		// Verifica correspondência exata primeiro
		if (pageTitles[pathname]) {
			return pageTitles[pathname];
		}

		// Verifica correspondências parciais para rotas dinâmicas
		if (pathname.startsWith("/dashboard/healthcare-professional/appointments")) {
			return "Agendamentos";
		}
		if (pathname.startsWith("/dashboard/healthcare-professional")) {
			return "Dashboard";
		}
		// Fallback padrão
		return "HealthPage";
	}

	// Título dinâmico baseado na rota atual
	const currentPageTitle = $derived(getPageTitle(page.url.pathname));
</script>

<header
	class="h-(--header-height) group-has-data-[collapsible=icon]/sidebar-wrapper:h-(--header-height) flex shrink-0 items-center gap-2 border-b transition-[width,height] ease-linear"
>
	<div class="flex w-full items-center gap-1 px-4 lg:gap-2 lg:px-6">
		<Sidebar.Trigger class="-ml-1" />
		<Separator orientation="vertical" class="mx-2 data-[orientation=vertical]:h-4" />
		<h1 class="text-base font-medium">{currentPageTitle}</h1>
		<div class="ml-auto flex items-center gap-2">
			<Button
				href="/"
				variant="default"
				size="sm"
				class="dark:text-foreground hidden sm:flex"
				target="_blank"
				rel="noopener noreferrer"
			>
				Assinar o Premium
			</Button>
		</div>
	</div>
</header>
